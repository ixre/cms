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
