# JR Cms .NET ! Open source .net cross platform cms.
# Version : 3.2
# Author : jarrysix(jarrysix@gmail.com)
# Date : 2018-12-22 08:02

# About setting inotify params see below: 
# https://github.com/mono/mono/issues/12535

# 引入基础镜像
FROM mono:slim
# 设置开发者
MAINTAINER jarrysix
# 设置标签
LABEL Vendor="jarrysix"
LABEL License="GPLv2"
LABEL Version=3.2

# 设置最大缓存数量
ENV MONO_ASPNET_WEBCONFIG_CACHESIZE 4096

WORKDIR /cms
RUN apt-get update -y && apt-get install -y --fix-missing wget unzip mono-fastcgi-server4  \
	   gcc libpcre3-dev libssl-dev zlib1g.dev make &&\
	echo "2.Install nginx" &&\
	wget -O - "https://nginx.org/download/nginx-1.17.0.tar.gz" | tar xzf - &&\
	cd nginx* && ./configure --prefix=/usr/local/nginx \
				--with-http_stub_status_module \
				--with-http_ssl_module \
				--with-http_flv_module --with-http_mp4_module \
				--with-http_gzip_static_module \
				--with-http_realip_module \
				&& make install && cd .. && rm -rf nginx-* 
		
RUN echo "3. Install cms program" \
	&& wget -O cms.zip http://s.to2.net/jrcms_latest?date=20190520 \
	&& unzip cms.zip && rm -f cms.zip \
	&& mkdir -p /var/cms && cp -r templates plugins oem /var/cms \
	&& rm -rf *server \
	&& echo "4. Clean" \
	&& apt-get remove -y gcc wget unzip libpcre3-dev libssl-dev zlib1g.dev make \
	&& apt-get autoremove -y \
	&& echo "5. Setting Envirment" \
	&& echo "fs.inotify.max_user_instances = 1638400\nfs.inotify.max_user_watches = 1638400" > /etc/sysctl.conf

# 复制本地文件到docker
COPY docker-boot.sh /
COPY nginx.conf /usr/local/nginx/conf

# 创建一个本地主机或其他容器的挂载点。
VOLUME ["/cms/config","/cms/templates","/cms/plugins",\
	"/cms/oem","/cms/uploads","/cms/data"]

# 暴露端口
EXPOSE 80

# 传递给启动命令的参数
CMD ["sh","/docker-boot.sh"]


# # Quick Start
# # ```
# # docker run --rm -it -p 8080:80 jarry6/jrcms
# # ```
# # open http://localhost:8080 in your brower.

# # Advance
# ```
# docker run --rm -p 8080:80 \
# 	--volume=$(pwd)/config:/cms/config \
#       --volume=$(pwd)/oem:/cms/oem \
# 	--volume=$(pwd)/templates:/cms/templates \
# 	--volume=$(pwd)/plugins:/cms/plugins \
# 	--volume=$(pwd)/uploads:/cms/uploads \
# 	--volume=$(pwd)/data:/cms/data \
# 	jarry6/jrcms
# ```

