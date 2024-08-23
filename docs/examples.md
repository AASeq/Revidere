# Examples

## Ping Google and CloudFlare DNS

Adds 4 ping checks, all based on IP address.

```yaml
checks:
  - kind: ping
    target: 8.8.8.8
    title: Google Primary
  - kind: ping
    target: 8.8.4.4
    title: Google Secondary
  - kind: ping
    target: 1.1.1.1
    title: CloudFlare Primary
  - kind: ping
    target: 1.0.0.1
    title: CloudFlare Secondary
```

Simplified variant using environment variable would be:
```
CHECKS=ping://8.8.8.8,ping://8.8.4.4,ping://1.1.1.1,ping://1.0.0.1
```


## Check Websites

Adds check for http://example.com site.

```yaml
checks:
  - kind: get
    target: https://example.com
    title: GET
  - kind: head
    target: https://example.com
    title: HEAD
```

Simplified variant using environment variable would be (note there is no option
to use HEAD):
```
CHECKS=https://example.com
```


## Random

Adds a random check and then a website status check based on it.

```yaml
checks:
  - kind: random
    title: Random
    name: random
  - kind: GET
    target: http://localhost:8089/healthz/random
    title: Random Health Probe
```

No exact equivalent for named checks when using just an environment variable.


## Check NTP Pool

Check NTP server website and ping given DNS name.

```yaml
checks:
  - kind: HEAD
    target: https://www.ntppool.org/en/
    title: Website
  - kind: Ping
    target: 0.pool.ntp.org
```

Simplified variant using environment variable uses GET:
```
CHECKS=https://www.ntppool.org/en/,ping://0.pool.ntp.org
```


## More Agressive Checks

Web check done every 5 seconds with 200 ms timeout. It takes 5 successful checks
to become healthy but only 2 to go unhealthy. Since check is named (`aaseq`),
its status can be checked at http://localhost:8089/healthz/aaseq.

And website title is customized.

```yaml
checks:
  - kind: HEAD
    target: https://www.aaseq.com/
    title: Website
    period: 5
    timeout: 0.2
    success: 5
    failure: 2
    name: aaseq
web:
  title: Status
  refresh: 5
```

No exact equivalent when using just an environment variable.


## Exact IP for web interface

Web prefix can be used to fix listener to a single IP address.

```yaml
web:
  prefix: http://192.168.1.1:8089/
```

No equivalent when using just an environment variable.


## Multiple prefixes

You can listen at multiple prefixes if you specify multiple prefixes. Example
below listens to both port 8089 and 8090.

```yaml
web:
  prefix:
    - http://*:8089/
    - http://*:8090/
```

No equivalent when using just an environment variable.


## More Expansive Logging

Increases log level for console to debug and file log level to verbose.

```yaml
logging:
  console: debug
  file:
    - level: verbose
    - path: /var/tmp/revidere.log
```


## Seq Logging

Log all information to seq in addition to console.

```yaml
logging:
  console: info
  seq:
    - level: info
    - url: http://localhost:5341
```
