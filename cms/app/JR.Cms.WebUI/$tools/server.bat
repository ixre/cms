@echo off
echo .NET调试运行工具(.NET 4.0)
echo.
echo 请在浏览器中输入以下地址访问,任意键结束服务。
echo 运行serverb.vbs后台启动
echo.

::代理端口
set port=80

::绑定主机(外网访问需要设定)
set host=localhost

::内网端口
set porJR=8000


cd ../
start /b $tools\server-proxy.exe -host %host% -port %port% -proxy http://localhost:%porJR%
start /b $tools\server_console.exe /a:./ /pm:Specific /port:%porJR%"