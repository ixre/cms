# JR-CMS

![Build Status](https://cloud.drone.io/api/badges/ixre/cms/status.svg)

基于.Net + DDD 构建的跨平台多站点开源内容管理系统; 同时支持`ASP.NET 4.x`和`.NET6.0`; 可以运行在 Windows,Linux,MacOSX 等操作系统；支持 Docker 容器。
此项目已维护超过十年, 不断使用最合适的技术改进. 独立服务器上建议运行`.NET6`版,或上传到虚拟主机。

￥ 99 元/年的虚拟主机也能开出多个网站, 推荐的虚拟主机参考:[主机服务商评测](#部署)

## 特性

- **跨平台**：支持 Windows、Linux、MacOSX 运行,同时支持虚拟主机。
- **支持容器**：提供容器镜像，可用 Docker/Podman 部署至服务器。
- **支援多种数据库**:支持 MySQL、SQLite、Sql Server、ACCESS 数据库, 推荐使用:MySQL 作为数据库。
- **领域驱动设计**：核心代码使用 DDD 领域驱动设计构建，通过领域模型，提供了可扩展性。
- **支持模板**：内置模板引擎，编写简单。后台支持模板的安装，网络安装，修改，备份等。
- **源代码编辑**：支持在线编辑代码，支持 EMMET 插件自动生成 HTML 代码。
- **支持插件**：支持网络安装插件，卸载插件等。利用插件可开发自定义功能。比如内嵌的采集系统。
- **多站点支持**：支持后台创建站点、域名绑定、虚拟目录等，站点相互隔离，大大节省服务器空间开支和维护成本。
- **内置 SEO 模块**:内置站内连接,URL 提供等 SEO 工具。

在线[演示站点](http://www.56x.net)-(运行于 CentOS7.2)

---

_感谢您看到这个页面，如果对您有帮助，或您对此感兴趣，请 star 或 fork 支持一下作者吧！_

## 快速开始

Windows 平台下，下载安装包[链接](http://s.56x.net/jrcms_latest), 运行命令启动服务:

```bash
dotnet JR.Cms.App.dll --urls http://+:8000
```

通过浏览器访问：<http://localhost:8000>

## 发布项目

打包发布需要环境如下：

- `.NET Standard 2.1`及以上(.NET6/.NET7)
- `.NET Framework 4.5.1`/`Mono`或以上

编译打包`.Net`程序包运行命令:

```bash
sh ./build.sh
```

编译打包`ASP.NET`程序包,运行命令:

```bash
sh ./aspnet_pack.sh
```

_注:在 windows 平台打包,需要使用`shell`客户端运行命令, 比如:`git-bash` 安装`git`会默认安装_

## 部署

### 一.　通过虚拟主机部署

- 需准备一台支持 ASP.NET 4.0 的虚拟主机。
- 下载程序文件: [jrcms-aspnet-latest.tar.gz](https://github.com/ixre/cms/releases) 并解压。
- 通过 FTP 上传 ASP.NET 版的所有文件到虚拟主机。
- 虚拟主机设置线程池为集成模式,版本更改为.NET4.0 及以上。

推荐虚拟主机服务商

- 西部数码: 工单速度处理快, 虚拟主机买二年送一年。
- 新网: 网络快,技术支持 24 小时在线处理。

### 二.　使用服务器或 VPS 部署

Windows(IIS)

- 点击下载[安装包](https://github.com/ixre/cms/releases) 并解压；
- 使用 IIS 添加站点，选择无托管代码, 完成部署。

Linux、MacOSX

```bash
curl -L https://github.com/ixre/cms/releases/download/v4.6/jrcms-latest.tar.gz | tar xz
cd cms && dotnet JR.Cms.App.dll --urls http://+:8080
```

浏览器访问: <http://127.0.0.1:8080>

### 　使用 Docker 容器运行

容器镜像托管在[docker.io](https://hub.docker.com/r/jarry6/cms), 操作步骤如下:

创建存放 CMS 模板、数据、插件、文件的目录:

```bash
mkdir /data/cms && cd /data/cms
```

运行容器：

```bash
#!/usr/bin/env sh
podman='podman';if [ $(whereis podman) = 'podman:' ]; then podman='docker';fi

mkdir -p mysql/conf.d mysql/data config templates plugins uploads oem root

$podman rm -f mysql-website
$podman rm -f cms

$podman run -d -p 3306:3306 --name mysql-website \
   -e MYSQL_ROOT_PASSWORD=123456 \
   -v $(pwd)/mysql/conf.d:/etc/mysql/conf.d \
   -v $(pwd)/mysql/data:/var/lib/mysql \
   --restart always mysql:8

$podman run -d  --name cms -p 8080:80 \
    --volume=$(pwd)/config:/cms/config \
    --volume=$(pwd)/data:/cms/data \
    --volume=$(pwd)/templates:/cms/templates \
    --volume=$(pwd)/plugins:/cms/plugins \
    --volume=$(pwd)/uploads:/cms/uploads \
    --volume=$(pwd)/oem:/cms/oem \
    --volume=$(pwd)/root:/cms/root \
    --restart always jarry6/cms:latest
```

### 通过 Nginx 反向代理

```nginx
server {
    listen       80;
    server_name     *.xxx.com;
    # SSL配置
    listen 443 default ssl;
    http2 on;
    ssl_certificate /etc/nginx/ssl/server.cer;
    ssl_certificate_key /etc/nginx/ssl/server.key;
    ssl_session_cache    shared:SSL:10m;
    ssl_session_timeout  5m;
    ssl_ciphers  HIGH:!aNULL:!MD5;
    ssl_prefer_server_ciphers  on;

    # gzip 配置
	gzip on;
    gzip_min_length  10k;
    gzip_comp_level 4;
    gzip_types      application/javascript text/css text/javascript image/jpeg image/gif image/png;

    client_max_body_size    10m;

    location / {
    proxy_pass      http://172.17.0.1:8081;
    proxy_set_header Host $host;
        proxy_set_header   X-Real-IP         $remote_addr;
        proxy_set_header   X-Forwarded-For   $proxy_add_x_forwarded_for;
        proxy_set_header   X-Forwarded-Proto $scheme;
    }
}

```

## 插件开发

详见：[https://github.com/jsix/cms/tree/master/plugins](https://github.com/jsix/cms/tree/master/plugins)

### 如何加入开发

请先在 github 上 fork 代码,克隆到本地修改后直接提交。 交流 QQ 群：737378973

### 捐助项目

如果项目对您有帮助, 可以购买虚拟主机向作者发起捐助. 如果您有购买的需要, 可以通过添加 QQ/微信:[959398298](tencent://message?uin=959398298)购买主机发起对项目的赞助, 我们同时给到`八折优惠`和额外的技术支持。
