# Configuration

## Configuration File

Configuration is contained in `config.yaml`, either in the same directory as
application or in `/config/config.yaml`.

While file needs to be a valid yaml file, quite permissive parsing is used and
thus attempt will be made to set non-parsable values to a sane default and/or
unrecognized elements will be ignored.

Example of config file is as follows:
```yaml
checks:
  - title: Always success
    kind: dummy
  - title: Random
    name: random
    kind: random
  - title: AASeq ping
    kind: ping
    target: aaseq.com
    timeout: 0.1
  - title: Random Health Probe
    kind: get
    target: http://localhost:8089/healthz/random
    success: 3
    failure: 2
    period: 20
    timeout: 0.1

web:
  title: Status
  refresh: 5

logging:
  console: verbose
  file:
    level: warning
    path: /var/log/revidere.log
  seq:
    level: information
    url: http://localhost:5341
```


### checks

This is a node containing a sequence. Each sequence member defines one health
check.


#### Common

Each member of sequence can contain the following keys:
* `kind`:    defines kind of health check; must be specified
* `target`:  defines target of a check; the exact syntax depends on the check
             kind
* `name`:    controls the name of check when using `healthz` path; each name must
             be unique and consists only of letter, number, dash (-), and
             underscore (_) characters
* `title`:   this key controls the title of the given check for the web
             interface; if value is not present it will default to either check
             name (if available) or check kind
* `period`:  interval between checks; must be between 1 second and 10 minutes;
             default is 10 seconds
* `timeout`: timeout for each check; it must be between 10 milliseconds and 10
             seconds; its value cannot be larger than period; default is 5
             seconds
* `success`: number of consecutive successful attempts before a target is
             considered healthy; must be between 1-10 seconds; default is 2
* `failure`: number of consecutive failed attempts before a healthy target is
             considered healthy; must be between 1-10 seconds; default is 2
* `visible`: if set to false, check will not be visible in GUI; default is true
* `break`:   if set to true, a new line will start after it

This node is mandatory.


### kind: ping

Key `target` is a mandatory and its value must be either hostname or ip that
ping request will be sent to.

Example:
```yaml
- kind: ping
  target: 1.1.1.1
```


### kind: get/head/post/put/delete

Key `target` is a mandatory and its value must be URL that request is sent to.
Kind matches the HTTP method to be used.

Example:
```yaml
- kind: get
  target: http://example.com
- kind: head
  target: http://example.net
```


#### kind: dummy

This kind is used for troubleshooting. It always returns true.

Example:
```yaml
- kind: dummy
```


#### kind: random

This kind is used for troubleshooting. It returns random health check outcomes.

The same `target` will result in the same random number seed. If target is not
specified, the current time will be used.

Example:
```yaml
- kind: random
```


### web

This node defines mappings for built-in web server. Its usage is optional.

Key `prefix` controls location where web server will be listening for requests.
By default this will be on port 8089 (`http://*:8089`). This key is optional.

Key `title` controls which title will webpage have (if custom title is
requested). This key is optional.

Key `refresh` controls how often page will refresh in a browser. Please note
that this only impacts web interface and not health checks. Default value is
`10` seconds. This key is optional.

Example:
```yaml
web:
  prefix: http://*:8089
  title: Status
  refresh: 10
```


### logging

This is a mapping node defining logging setup.

This node is optional.


### logging (Common)

Each `logging` subnode has `level` key in common.

Key `level` can contain the following values:
* `none`: no logging will be performed
* `verbose`: full logging is enabled
* `debug`: debug logging is enabled
* `information`: informational messages will be shown
* `warning`: warning messages not impacting system health
* `error`: issues having a significant impact to the application
* `panic`: unrecoverable application errors

Note that each level implicitly also includes higher severity levels, e.g. using
`warning` will print `warning`, `error`, and `panic`.


#### logging/console

Mappings in this node define logging properties for console output. If there is
no `console` node, application will default to using `information` console level
logging.

Example:
```yaml
logging:
  console:
    level: verbose
```

For console node only, this can be shortened:
```yaml
logging:
  console: verbose
```


#### logging/file

Mappings in this node define logging properties for file output. If not
specified, default log `level` of `debug` is used.

Key `path` must contain the name of the log file.

Key `interval` defines rolling interval for the file and can be one of the
following values:
* `infinite`: no file rolling will occur
* `year`: file will rollover once a year
* `month`: file will rollover once a month
* `day`: file will rollover once a day (default)
* `hour`: file will rollover once a hour
* `minute`: file will rollover once a minute

Key `retain` controls how many files will be retained after rollover. Default is
`7`.

Key `buffered` controls if buffering will be used. Default is `true`.

Example:
```yaml
logging:
  file:
    level: warning
    path:  /var/log/revidere.log
    interval: hour
    retain: 24
```


#### logging/seq

Mappings in this node define logging properties for [seq](https://datalust.co/seq)
output. If not specified, default log `level` of `info` is used.

Key `url` must contain the server accepting the log messages.

Example:
```yaml
logging:
  seq:
    level: warning
    url: http://localhost:5341
```


## Environment Variable

Limited configuration can be done using environment variables. This is not
intended for long-term usage.

### CHECKS

Health checks can be specified in comma-separated list. Each health check target
will use its `kind` as a prefix and its `target` as host/path parameters. No
custom configuration will be available and for HTTP checks, only GET method is
supported.

Example:
```env
CHECKS=ping://8.8.4.4,http://aaseq.com
```
