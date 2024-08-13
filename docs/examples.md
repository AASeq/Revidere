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
