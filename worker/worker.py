#!/usr/bin/env python
import argparse
import sys

from dns import reversename, resolver
from flask import Flask, abort, jsonify
from ping3 import ping
from werkzeug.sansio.response import Response

import json
import requests
import whoisit

app = Flask(__name__)
app.config.from_object(__name__)

tasks = ['ping', "geoip", "dns", "rdap"]


def DoPing(address):
    delay_s = ping(address, timeout=2)

    retval = {
        'Task': 'Ping',
        'Alive': False,
        'Delay': 0
    }

    if delay_s is not None:
        retval['Alive'] = True
        retval['Delay'] = delay_s

    return jsonify(retval)


def DoDNS(address):
    parts = address.split('.')

    # Rough AP parsing
    if len(parts) == 4:
        addr = reversename.from_address(address)
        retval = {
            'Task': 'DNS',
            'Name': str(resolver.resolve(addr, "PTR")[0])
        }
        return jsonify(retval)

    retval = {
        'Task': 'DNS',
        'A_Records': [],
        'MX_Records': [],
        'CNAME_Records': []
    }

    try:
        a_records = resolver.resolve(address, 'A')
        for a in a_records:
            retval['A_Records'].append(str(a))
    except Exception:
        retval['A_Records'] = None

    try:
        mx_records = resolver.resolve(address, 'MX')
        for mx in mx_records:
            retval['MX_Records'].append(str(mx))
    except Exception:
        retval['MX_Records'] = None

    try:
        cname_records = resolver.resolve(address, 'CNAME')
        for cname in cname_records:
            retval['CNAME_Records'].append(str(cname))
    except Exception:
        retval['CNAME_Records'] = None

    return jsonify(retval)


def DoRDAP(address):
    retval = {
        'Task': 'RDAP',
    }

    if not whoisit.bootstrap():
        retval['Status'] = 'Bootstrap failed'
        return jsonify(retval)

    parts = address.split('.')

    # FIXME: whoisit result doesn't want to be serialized
    if len(parts) == 4:
        retval['rst'] = str(whoisit.ip(address))
    else:
        retval['rst'] = str(whoisit.domain(address))

    return jsonify(retval)


def DoGeoIP(address):
    req_url = "https://geolocation-db.com/jsonp/" + address
    resp = requests.get(req_url)
    result = resp.content.decode()
    result = result.split("(")[1].strip(")")
    result  = json.loads(result)
    result['Task'] = 'GeoIP'
    return result


@app.route('/<addr>/<task>')
def HandleTask(addr, task):
    task = task.lower()

    # Unsupported task, return Bad Request
    if task not in tasks:
        abort(400)

    if task == 'ping':
        return DoPing(addr)
    if task == 'dns':
        return DoDNS(addr)
    if task == 'rdap':
        return DoRDAP(addr)
    if task == 'geoip':
        return DoGeoIP(addr)


def start_worker():
    parser = argparse.ArgumentParser(description='Worker node for IpFinder2')
    parser.add_argument('--port', type=int, help='server port', default="2112")
    args = parser.parse_args()

    app.run(host="0.0.0.0", port=args.port)


if __name__ == '__main__':
    sys.exit(start_worker())