# AtNet.CMS #

基于asp.net mvc + DDD 构架的开源.net cms系统.

## 特性：##

### 1. 跨平台 ###
支持Windows、Linux、MacOX运行。linux运行案例：http://blog.ops.cc
### 2. DDD领域驱动 ###
使用领域驱动设计构建，通过简单的领域模型，提供了强大扩展能力。
### 3. 支援多种数据库 ###
默认使用SQLite作为数据库，同时支持Sql Server、MySQL、OLEDB(ACCESS)
### 4. 支持模板引擎 ###
内嵌一个简单的模板引擎实现，可以用html作为呈现主体。后台支持模板的安装，网络安装，修改，备份等。
### 5. 插件内核 ###
支持插件，支持网络安装插件，卸载插件等。利用插件可开发自定义功能。比如内嵌的采集系统。
### 6. 集成Ironpython ###
可以利用python进行插件开发，并提供了python2.7的标准包。
### 7. 多站点支持 ###
支持后台创建站点，站点之间相互独立。大大节省服务器空间开支和维护成本。
### 8. 基于左右值的分类算法 ###
基于左右值分类算法，实现了真正意义上的无限分类。
### 9. 美观的UI ###

## 部署 ##
### LINUX平台 ###
        wget -nd http://z3q.net/j6cms_latest
        unzip cms_release_latest.zip
        fastcgi-mono-server4 /applications=/:cms /socket=tcp:127.0.0.1:8080
浏览器访问: http://127.0.0.1:8080

### WINDOWS平台 ###
测试环境下，可直接运行$tools/server.bat
正式环境请配置IIS

## 插件开发 ##
详见：github.com/jsix/cms/tree/master/plugin

## 如何加入开发 ##
交流QQ群：306064037



