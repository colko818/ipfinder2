#!/usr/bin/env python
import argparse
import subprocess
import sys

from sysconfig import get_platform

from flask import Flask, abort

app = Flask(__name__)
app.config.from_object(__name__)

tasks = ['ping',]


def DoPing(address):
    cnt = '-n'
    if get_platform().startswith('win'):
        cnt = '-n'
    else:
        cnt = '-c'

    cmd = ['ping', cnt, '1', '-w', '500', str(address)]
    print(cmd)

    return subprocess.Popen(cmd, stdout=subprocess.PIPE).communicate()[0]


@app.route('/<addr>/<task>')
def HandleTask(addr, task):
    if task not in tasks:
        abort(400)

    if task == 'ping':
        ret = DoPing(addr)


        return ret


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