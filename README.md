# JR.CMS #

基于Asp.net mvc + DDD 构架的开源.net cms系统， 支持Windows,Linux,MacOSX等操作系统；支持Docker容器。

## 特性：

- **跨平台**：支持Windows、Linux、MacOSX运行。
- **支持容器**：提供Docker镜像，能使用Docker部署。
- **支援多种数据库**:支持MySQL、SQLite、Sql Server、ACCESS数据库, 推荐使用:MySQL作为数据库。
- **领域驱动设计**：核心代码使用DDD领域驱动设计构建，通过领域模型，提供了可扩展性。
- **支持模板**：内置模板引擎，编写简单。后台支持模板的安装，网络安装，修改，备份等。
- **支持插件**：支持网络安装插件，卸载插件等。利用插件可开发自定义功能。比如内嵌的采集系统。
- **多站点支持**：支持后台创建站点、域名绑定、虚拟目录等，站点相互隔离，大大节省服务器空间开支和维护成本。

## 部署 ##
安装包下提供一个简易的Server, 在Windows下进入目录【$server】，运行【server.bat】,

通过浏览器访问：http://localhost:8000

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

## Docker ##
创建存放CMS模板、数据、插件、文件的目录:
```
mkdir /data/cms && cd /data/cms
```

运行容器：
```
docker run -d -p 80:80 --volume=$(pwd)/config:/cms/config \
    --volume=$(pwd)/data:/cms/data \
    --volume=$(pwd)/templates:/cms/templates \
    --volume=$(pwd)/plugins:/cms/plugins \
    --volume=$(pwd)/uploads:/cms/uploads \
    --restart always jarry6/jrcms
```

# 二次开发

### 打包发布核心类库 ###
在项目生成事件-》后期生成事件命令行中输入：

	cd $(SolutionDir)../../cmd/
	./build_win32.bat

## 插件开发 ##
详见：github.com/newmin/cms/tree/master/plugin



## 如何加入开发 ##

请先在github上fork代码,克隆到本地修改后直接提交。
交流QQ群：306064037 , QQ:959398298
