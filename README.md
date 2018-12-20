# JR.CMS #

基于Asp.net mvc + DDD 构架的开源.net cms系统， 支持Windows,Linux,MacOSX等操作系统；支持Docker容器。

## 特性：##

- 跨平台：支持Windows、Linux、MacOSX运行。
- 支持容器：提供Docker镜像，能使用Docker部署。
- 领域驱动设计：使用DDD领域驱动设计构建，通过领域模型，提供了可扩展性。
- 支援多种数据库:支持MySQL、SQLite、Sql Server、ACCESS数据库, 推荐使用:MySQL作为数据库，
- 模板：内置模板引擎，编写简单。后台支持模板的安装，网络安装，修改，备份等。
- 支持插件：支持网络安装插件，卸载插件等。利用插件可开发自定义功能。比如内嵌的采集系统。
- 多站点支持：支持后台创建站点、域名绑定、虚拟目录等，站点相互隔离，大大节省服务器空间开支和维护成本。

## 部署 ##
### LINUX平台 ###
        wget -nd http://t.to2.net/jrcms_latest
        unzip cms_release_latest.zip
        fastcgi-mono-server4 /applications=/:cms /socket=tcp:127.0.0.1:8080
	
浏览器访问: http://127.0.0.1:8080

### WINDOWS平台 ###
测试环境下，可直接运行$tools/server.bat
正式环境请配置IIS

### 打包发布核心类库 ###
在项目生成事件-》后期生成事件命令行中输入：

	cd $(SolutionDir)../../cmd/
	./build_win32.bat

## 插件开发 ##
详见：github.com/newmin/cms/tree/master/plugin



## 如何加入开发 ##

请先在github上fork代码,克隆到本地修改后直接提交。
交流QQ群：306064037
