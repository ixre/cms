# JR Cms .NET ! Open source .net cross platform cms.
# Version : 3.2
# Author : jarrysix(jarrysix@gmail.com)
# Date : 2020-03-22 08:02

# How to docked a dotnet app: 
# https://docs.docker.com/engine/examples/dotnetcore/



FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
ENV RELEASE_DIR /app/out/release
WORKDIR /app
COPY . ./
WORKDIR src/JR.Cms.App
RUN dotnet restore && dotnet publish -c Release -o ${RELEASE_DIR}
RUN mkdir -p ${RELEASE_DIR}/root && cp -r root/*.md ${RELEASE_DIR}/root && \
    mkdir -p ${RELEASE_DIR}/templates && cp -r templates/default ${RELEASE_DIR}/templates && \
    cp -r public oem install plugins ${RELEASE_DIR} && \
    cd ${RELEASE_DIR} && \
    rm -rf *.pdb *.xml appsettings.json appsettings.Development.json && \
    rm -rf runtimes/win* runtimes/osx* runtimes/*arm* runtimes/*x86 && \
    cp ../../LICENSE ../../README.md . && ls -al

# 设置开发者
MAINTAINER jarrysix
# 设置标签
LABEL Vendor="jarrysix"
LABEL License="GPLv2"
LABEL Version=4.0

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:5.0-alpine
ENV CMS_RUN_ON_DOCKER yes
WORKDIR /cms
COPY --from=build-env /app/out/release ./
RUN sed -i 's/dl-cdn.alpinelinux.org/mirrors.aliyun.com/g' /etc/apk/repositories && \
    apk add libgdiplus --update-cache --repository http://dl-3.alpinelinux.org/alpine/edge/testing/ --allow-untrusted && \
    apk add tzdata fontconfig ttf-dejavu && \
    cp /usr/share/zoneinfo/Asia/Shanghai /etc/localtime && apk del tzdata \
    && echo "create data init folder.." && \
    mkdir -p ${CMS_INIT_DIR:=/var/cms} && mv -f templates plugins oem root ${CMS_INIT_DIR} && \
    echo "if [ \`ls /cms/templates|wc -w\` -eq 0 ];then cp -r ${CMS_INIT_DIR}/templates/* /cms/templates;fi;" \
         "if [ \`ls /cms/plugins|wc -w\` -eq 0 ];then cp -r ${CMS_INIT_DIR}/plugins/* /cms/plugins;fi;"\
         "if [ \`ls /cms/oem|wc -w\` -eq 0 ];then cp -r ${CMS_INIT_DIR}/oem/* /cms/oem;fi;"\
         "dotnet JR.Cms.App.dll --urls http://+:80" > ../entrypoint.sh && chmod u+x ../entrypoint.sh

VOLUME ["/cms/config","/cms/templates","/cms/plugins",\
        "/cms/uploads","/cms/data","/cms/root","/cms/oem"]
        
EXPOSE 80

ENTRYPOINT ["sh","../entrypoint.sh"]

# # Quick Start
# # ```
# # docker run --rm -it -p 8080:80 jarry6/cms
# # ```
# # open http://localhost:8080 in your brower.

# # Advance
# ```
# docker run --rm -p 8080:8080 \
# 	--volume=$(pwd)/config:/cms/config \
#   --volume=$(pwd)/oem:/cms/oem \
# 	--volume=$(pwd)/templates:/cms/templates \
# 	--volume=$(pwd)/plugins:/cms/plugins \
# 	--volume=$(pwd)/uploads:/cms/uploads \
# 	--volume=$(pwd)/data:/cms/data \
#   --volume=$(pwd)/root:/cms/root \
# 	jarry6/cms
# ```

