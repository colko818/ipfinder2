#!/usr/bin/env python
import argparse
import subprocess
import sys

from sysconfig import get_platform

from dns import reversename, resolver
from flask import Flask, abort
from ping3 import ping

import whoisit

app = Flask(__name__)
app.config.from_object(__name__)

tasks = ['ping', "geoip", "dns", "rdap"]


def DoPing(address):
    delay_s = ping(address, timeout=2)

    if delay_s is not None:
        return "Ping:\nHost is alive! {0:.3g}s delay".format(delay_s)

    return "Ping:\nHost is dead/timedout :("


def DoDNS(address):
    retval = 'DNS:\n'
    parts = address.split('.')

    # Rough AP parsing
    if len(parts) == 4:
        addr = reversename.from_address(address)
        return retval + str(resolver.resolve(addr, "PTR")[0])

    try:
        a_records = resolver.resolve(address, 'A')
        for a in a_records:
            retval = retval + "A: {}\n".format(str(a))
    except Exception:
        retval = retval + "No A records\n"

    try:
        mx_records = resolver.resolve(address, 'MX')
        for mx in mx_records:
            retval = retval + "MX: {}\n".format(str(mx))
    except Exception:
        retval = retval + "No MX records\n"

    try:
        cname_records = resolver.resolve(address, 'CNAME')
        for cname in cname_records:
            retval = retval + "CNAME: {}\n".format(str(cname))
    except Exception:
        retval+= "No CNAME records\n"

    return retval


def DoRDAP(address):
    if not whoisit.bootstrap():
        return "RDAP init failed"

    parts = address.split('.')

    if len(parts) == 4:
        return str(whoisit.ip(address))

    return str(whoisit.domain(address))


def DoGeoIP(address):
    return ""


@app.route('/<addr>/<task>')
def HandleTask(addr, task):
    task = task.lower()
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
    parser.add_argument('--port', type=int, help='server port')
    args = parser.parse_args()

    port = args.port
    if port is None:
        port=2112

    app.run(host="0.0.0.0", port=port)


if __name__ == '__main__':
    sys.exit(start_worker())