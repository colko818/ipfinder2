# Manager

A dotnet webapi application in C# which serves as the entry point to a cluster
of ipfinder worker nodes.

The API has one endpoint `/api/Manager/request` which accepts an object with
an address (IP or domain) and a list of network related commands to run.

If the commands list is left blank, a default list is used. Default: `{ "ping", "dns", "rdap", "geoip" }`. The manager does not validate that a given command is valid, it simply passes the command on to the worker. If the worker does not support a given command, it responds with a `400` bad request. This will not prevent other valid commands from running.

### Sample Request

```
POST /api/Manager/request
{
    "address": "8.8.8.8",
    "commands": [ "ping", "dns", "rdap", "geoip" ]
}
```

### Partial results on invalid command

```
{
  "address": "collins-sibley.com",
  "commands": [
    "ping", "png", "dns"
  ]
}
```

```
Results in: 0.439s


{"Alive":true,"Delay":0.22495818138122559,"Task":"Ping"}


Error: 80131500 
Message: Response status code does not indicate success: 400 (BAD REQUEST).

{"A_Records":["199.34.228.159"],"CNAME_Records":null,"MX_Records":["10 mx1.privateemail.com.","10 mx2.privateemail.com."],"Task":"DNS"}
```

## Docker

The Manager node is avaiable on Docker Hub [here.](https://hub.docker.com/repository/docker/gaella818/manager-node)

## Future Work

* Authentication on request
* Test framework
* A Frontend user interface (not swagger)
* Error handling if no workers are connected to the cluster.
* Proper JSON response
* Configure host & port from program entry