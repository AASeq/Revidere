# Overview

Revidere is a simple health checker with built-in web interface for easy
semaphore-style monitoring.

By default its status web page is available at http://localhost:11080/ with a
health check URL for each status at [http://localhost/healthz/`CHECK_NAME`/](http://localhost/healthz/).


## Docker: A Quick Version

You can run a [docker container](https://hub.docker.com/repository/docker/aaseq/revidere/)
by redirecting port `11080` and adding all desired checks in `CHECKS` environment
variable (comma-separated).

```bash
docker run -d -p 11080:11080 -e CHECKS=ping://1.1.1.1 --restart always --name revidere aaseq/revidere:latest
```

Please note this is a simplified version and for the full configuration you need
to use `/config/config.yaml` file.


## Docker: COnfiguration File

To fully customize Revidere, it's necessary to use config file.

```bash
docker run -d -p 11080:11080 -v /home/user/config.yaml:/config/config.yaml --restart always --name revidere aaseq/revidere:latest
```


## Other

For more information, check [documentation](https://aaseq.com/revidere/).

Source at [GitHub](https://github.com/aaseq/revidere).
