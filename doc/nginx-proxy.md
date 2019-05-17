## 使用Nginx对系统进行反向代理 ##
一、简单方式
```
server {
        listen          80;
        server_name     www.xxx.com;
        client_max_body_size    10m; 
        
        location / {
                proxy_pass          http://localhost:8080;
                proxy_set_header    Host $host;
                proxy_set_header    X-Real-IP        $remote_addr;
                proxy_set_header    X-Forwarded-For  $proxy_add_x_forwarded_for;
        }
}
```
二、NGINX 配置SSL，并自动跳转到HTTPS
```
server {
        listen          80;
        listen          443 ssl;
        server_name     www.xxx.com;
        client_max_body_size    10m;  
        ssl_certificate /data/ssl/xxx_server.cer;
        ssl_certificate_key /data/ssl/xxx_server.key;

        set $h 0;
        if ($server_port !~ 443){set $h 1;}
            if ($host = xxx.com){set $h "${h}1";}
        if ($host = www.xxx.com){set $h "${h}1";}
        if ($h = "11"){
          rewrite ^(/.*)$ https://www.xxx.com$1 permanent;
        }
        location / {
                proxy_pass          http://host:8080;
                proxy_set_header    Host $host;
                proxy_set_header    X-Real-IP        $remote_addr;
                proxy_set_header    X-Forwarded-For  $proxy_add_x_forwarded_for;
        }
}
```
