# IpFinder2
IP or domain lookup API

## Manager

A dotnet webapi application in C# which serves as the entry point to a cluster of ipfinder worker nodes.

[Documentation](manager/README.md)


## Worker

A simple python worker to be replicated on k8 cluster.

[Documentation](worker/README.md)

## Deployment

ipfinder2 is deployed on AWS and is accessable [here.]()