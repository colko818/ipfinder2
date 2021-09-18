docker pull gaella818/worker-node:0.1
docker pull gaella818/manager-node:0.2.1

docker kill worker_node manager_node
docker rm worker_node manager_node

docker run -d -p 2112:2112 --name worker_node gaella818/worker-node:0.1
docker run -d -p 5000-5001:5000-5001 --name manager_node gaella818/manager-node:0.2.1

docker network create ipfinder
docker network connect ipfinder worker_node
docker network connect ipfinder manager_node