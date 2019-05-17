# JR.CMS #

基于Asp.net mvc + DDD 构建的跨平台开源内容管理系统， 可以运行在Windows,Linux,MacOSX等操作系统；支持Docker容器。

在线[演示网站](http://www.cms.to2.net)-(运行于CentOS7.2)

------------------------------------------------------------------------
*感谢您看到这个页面，如果对您有帮助，或您对此感兴趣，请star或fork支持一下作者吧！*

## 特性：

- **跨平台**：支持Windows、Linux、MacOSX运行。
- **支持容器**：提供Docker镜像，能使用Docker部署。
- **支援多种数据库**:支持MySQL、SQLite、Sql Server、ACCESS数据库, 推荐使用:MySQL作为数据库。
- **领域驱动设计**：核心代码使用DDD领域驱动设计构建，通过领域模型，提供了可扩展性。
- **支持模板**：内置模板引擎，编写简单。后台支持模板的安装，网络安装，修改，备份等。
- **源代码编辑**：支持在线编辑代码，支持EMMET插件自动生成HTML代码。
- **支持插件**：支持网络安装插件，卸载插件等。利用插件可开发自定义功能。比如内嵌的采集系统。
- **多站点支持**：支持后台创建站点、域名绑定、虚拟目录等，站点相互隔离，大大节省服务器空间开支和维护成本。

## 快速开始
Windows平台下，下载安装包[链接](http://s.to2.net/jrcms_latest), 解压后，进入目录【$server】，运行【server.bat】,

通过浏览器访问：http://localhost:8000

## 发布项目
打包发布需要环境如下：
- Visual Studio 2013 及以上
- .NET Framework 3.5
- Git 2.20及以上
```   
a. 进入cms打开解决方案并编译
b. 进入bootstrap 打开解决方案并编译
c. 运行build.sh 进行打包。
```
_打包后的完整包存放于dist目录, 更新包存放于dist/update目录_

## 部署 ##

### 虚拟主机 ###
1. 需准备一台支持ASP.NET 4.0的虚拟主机

2. 使用FTP上传完成部署。

### Windows ###
1. 点击[下载](http://s.to2.net/jrcms_latest)安装包，并解压；

2. 使用IIS添加站点，IIS需开启ASP.NET 4.0及以上功能。

### LINUX、MacOSX ###
```
wget -O cms.zip http://s.to2.net/jrcms_latest && unzip cms.zip
fastcgi-mono-server4 /applications=/:cms /socket=tcp:127.0.0.1:8080
```
浏览器访问: http://127.0.0.1:8080

*在Linux及MacOSX上运行，需先安装mono

### 反向代理 ###
- [通过Nginx反向代理](doc/nginx-proxy.md)
- [通过Apache反向代理](doc/apache-proxy.md)

## Docker ##

[Docker镜像帮助](https://hub.docker.com/r/jarry6/jrcms)

1. 创建存放CMS模板、数据、插件、文件的目录:
```
mkdir /data/cms && cd /data/cms
```

2. 运行容器：
```
docker run -d -p 80:80 --volume=$(pwd)/config:/cms/config \
    --volume=$(pwd)/data:/cms/data \
    --volume=$(pwd)/templates:/cms/templates \
    --volume=$(pwd)/plugins:/cms/plugins \
    --volume=$(pwd)/uploads:/cms/uploads \
    --restart always jarry6/jrcms
```

# 插件开发 #
详见：[https://github.com/jsix/cms/tree/master/plugins](https://github.com/jsix/cms/tree/master/plugins)

## 如何加入开发 ##

请先在github上fork代码,克隆到本地修改后直接提交。
交流QQ群：737378973 , QQ:959398298
