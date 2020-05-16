#!/usr/bin/env sh

# init data
if [ `ls /cms/templates|wc -w` -eq 0 ];then 
	cp -r /var/cms/templates/* /cms/templates
fi
if [ `ls /cms/plugins|wc -w` -eq 0 ];then 
	cp -r /var/cms/plugins/* /cms/plugins
fi
if [ `ls /cms/oem|wc -w` -eq 0 ];then
        cp -r /var/cms/oem/* /cms/oem
fi

/usr/local/nginx/sbin/nginx && fastcgi-mono-server4 /applications=/:/cms \
	/multiplex=True /socket=unix:/var/cms/cms.sock /printlog=True

#sleep 5s && chmod o+rwx /var/cms/cms.sock && /usr/local/nginx/sbin/nginx 

echo "[ JRCms][ OK]: jrcms started successfully!"




