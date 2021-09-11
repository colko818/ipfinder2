#!/usr/bin/env python
import argparse
import subprocess
import sys

from flask import Flask

app = Flask(__name__)
app.config.from_object(__name__)

@app.route('/ping/<addr>')
def Ping(addr):
    return subprocess.Popen([
        'ping',
        '-c', '1',
        '-w', '500',
        str(addr)], stdout=subprocess.PIPE).communicate()[0]


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