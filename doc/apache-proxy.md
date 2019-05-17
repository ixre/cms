## 使用Apache对系统进行反向代理  ##
1. 编辑httpd.ini，取消以下模块的注视
```
LoadModule proxy_module modules/mod_proxy.so
LoadModule proxy_connect_module modules/mod_proxy_connect.so
LoadModule proxy_http_module modules/mod_proxy_http.so
```
2、编辑vhosts.conf文件，添加主机
```
<VirtualHost *:80>
ServerName www.xxx.com
ProxyPass / http://localhost:8080/
ProxyPassReverse / http://localhost:8080/
ProxyPreserveHost on
</VirtualHost>
```
3、重启Apache生效

_注：代理地址后必须包含"/"，否则Apache无法启动_
