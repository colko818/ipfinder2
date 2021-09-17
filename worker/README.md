# Worker Node

A simple python worker to be replicated on k8 cluster.

I went with python for the worker node to keep things simple, executing network commands like `ping` are easiest with python.

## Docker

The worker node is available on Docker Hub [here.](https://hub.docker.com/repository/docker/gaella818/worker-node)

## Future Work
* Address validation (I would probably do this on the manager because I could do it once as the request comes in rather than doing it on each worker every time)
* Support more commands: VirusTotal, open ports, website status, domain availability, etc
* Testing