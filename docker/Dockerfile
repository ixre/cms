# JR Cms .NET ! Open source .net cross platform cms.
# Version : 3.2
# Author : jarrysix(jarrysix@gmail.com)
# Date : 2020-03-22 08:02

# How to docked a dotnet app: 
# https://docs.docker.com/engine/examples/dotnetcore/

FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build-env
WORKDIR /app
COPY src ./
RUN dotnet restore
RUN dotnet publish -c Release -o out && ls -al

# 设置开发者
MAINTAINER jarrysix
# 设置标签
LABEL Vendor="jarrysix"
LABEL License="GPLv2"
LABEL Version=3.2


# 创建一个本地主机或其他容器的挂载点。
VOLUME ["/cms/config","/cms/templates","/cms/plugins",\
	"/cms/oem","/cms/uploads","/cms/data"]

# 暴露端口
EXPOSE 80

# 传递给启动命令的参数
ENTRYPOINT ["dotnet", "JR.Cms.App.dll"]

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

