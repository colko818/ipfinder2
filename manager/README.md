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

## Full Report

```
{
  "address": "collins-sibley.com",
  "commands": [
    "ping", "dns", "rdap", "geoip"
  ]
}
```

```
Results in: 2.384s


{"Alive":true,"Delay":0.09172511100769043,"Task":"Ping"}


{"A_Records":["199.34.228.159"],"CNAME_Records":null,"MX_Records":["10 mx1.privateemail.com.","10 mx2.privateemail.com."],"Task":"DNS"}


{"Task":"RDAP","rst":"{'handle': '2539207615_DOMAIN_COM-VRSN', 'parent_handle': '', 'name': 'COLLINS-SIBLEY.COM', 'whois_server': '', 'type': 'domain', 'terms_of_service_url': 'https://www.verisign.com/domain-names/registration-data-access-protocol/terms-service/index.xhtml', 'copyright_notice': '', 'description': [], 'last_changed_date': None, 'registration_date': datetime.datetime(2020, 6, 17, 15, 55, 16, tzinfo=tzutc()), 'expiration_date': datetime.datetime(2022, 6, 17, 15, 55, 16, tzinfo=tzutc()), 'url': 'https://rdap.verisign.com/com/v1/domain/COLLINS-SIBLEY.COM', 'rir': '', 'entities': {'registrar': [{'handle': '9', 'type': 'entity', 'name': 'Register.com, Inc.'}]}, 'nameservers': ['DNS1.REGISTER.COM', 'DNS2.REGISTER.COM'], 'status': ['client transfer prohibited']}"}


{"IPv4":"107.77.192.198","Task":"GeoIP","city":null,"country_code":"US","country_name":"United States","latitude":42.4645,"longitude":-83.3763,"postal":null,"state":"Michigan"}

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