# QuickStart
```
docker run --rm -it -p 8080:80 jarry6/jrcms
```
Open http://localhost:8080 in your brower.

# Mount data volume

You can save plugin、template、config file,data file in your volumes.

```
cd /data/cms
docker run -d -p 80:80 --volume=$(pwd)/config:/cms/config \
  --volume=$(pwd)/data:/cms/data --volume=$(pwd)/templates:/cms/templates \
  --volume=$(pwd)/plugins:/cms/plugins --volume=$(pwd)/uploads:/cms/uploads \
  --restart always jarry6/jrcms
```

# Add nginx proxy
```
server {
    listen                80;
    server_name           localhost;
    client_max_body_size  1024m;
    location / {
        proxy_pass   http://host:8081;
        proxy_set_header Host $host;
        proxy_set_header   X-Real-IP        $remote_addr;
        proxy_set_header   X-Forwarded-For  $proxy_add_x_forwarded_for;
    }
}
```
