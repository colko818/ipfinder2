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

I am learning AWS as I go. This is where I am at currently with deployment. I am
attempting to figure out AWS ingress to get the application accessible from the
internet.


```
ubuntu@ip-172-31-38-54:~$ kubectl get all
NAME                                           READY   STATUS    RESTARTS      AGE
pod/manager-node-deployment-77c4c694bb-7mssd   1/1     Running   0             59m
pod/worker-node-deployment-7c99767b84-bt6dd    1/1     Running   0             67m
pod/worker-node-deployment-7c99767b84-fsggk    1/1     Running   1 (63m ago)   67m
pod/worker-node-deployment-7c99767b84-nbjh7    1/1     Running   0             67m
pod/worker-node-deployment-7c99767b84-wfk8q    1/1     Running   1 (63m ago)   67m

NAME                           TYPE           CLUSTER-IP       EXTERNAL-IP   PORT(S)          AGE
service/kubernetes             ClusterIP      10.96.0.1        <none>        443/TCP          70m
service/manager-node-service   LoadBalancer   10.102.231.171   <pending>     5001:30000/TCP   5m20s
service/worker-node-service    ClusterIP      10.108.210.4     <none>        2112/TCP         67m

NAME                                      READY   UP-TO-DATE   AVAILABLE   AGE
deployment.apps/manager-node-deployment   1/1     1            1           59m
deployment.apps/worker-node-deployment    4/4     4            4           67m

NAME                                                 DESIRED   CURRENT   READY   AGE
replicaset.apps/manager-node-deployment-77c4c694bb   1         1         1       59m
replicaset.apps/worker-node-deployment-7c99767b84    4         4         4       67m
ubuntu@ip-172-31-38-54:~$ kubectl get nodes
NAME               STATUS   ROLES                  AGE   VERSION
ip-172-31-38-54    Ready    control-plane,master   71m   v1.22.2
ip-172-31-44-208   Ready    <none>                 69m   v1.22.2
ip-172-31-44-30    Ready    <none>                 68m   v1.22.2
```