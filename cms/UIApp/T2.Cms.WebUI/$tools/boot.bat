::代理端口
set port=80

::绑定主机(外网访问需要设定)
set host=localhost

::代理网址
set proxy=http://www.j6.cc


cd ../
start /b $tools\server-proxy.exe -host %host% -port %port% -proxy %proxy%