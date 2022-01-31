# Elasticstack Docker Config
Make sure Docker Engine is allotted at least 4GiB of memory. In Docker Desktop, you configure resource usage on the Advanced tab in Preference (macOS) or Settings (Windows).
1. Run docker-compose to bring up the three-node Elasticsearch cluster and Kibana: `docker-compose up`

2. Submit a _cat/nodes request to see that the nodes are up and running: `curl -X GET "localhost:9200/_cat/nodes?v&pretty"`

3. Open Kibana to load sample data and interact with the cluster: http://localhost:5601.

When you’re done experimenting, you can tear down the containers and volumes by running `docker-compose down -v`.

#IMPORTANT

Make sure to set the `max_map_count`

## Windows and macOS with Docker Desktop

The vm.max_map_count setting must be set via docker-machine:
```powershell
docker-machine ssh
sudo sysctl -w vm.max_map_count=262144
```

## Windows with Docker Desktop WSL 2 backend

The vm.max_map_count setting must be set in the docker-desktop container:
```powershell
wsl -d docker-desktop
sysctl -w vm.max_map_count=262144
```