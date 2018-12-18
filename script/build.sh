#!/usr/bin/env sh

echo "======================================="
echo "= JR Cms .NET ! assembly ="
echo "======================================="

echo scanning assemblies...

cur=$(pwd)

cd ../bin

echo "1. build dll "

#set megdir=%dir%\dist\bin\
#if exist "%cur%/merge.exe" (
#cd %dir%/src/bin/
#echo  /keyfile:%dir%\src\JR.cms.snk>nul

${cur}/merge.exe -closed -ndebug \
	/keyfile:../cms/jr.cms.snk \
	/targetplatform:v4 /target:dll /out:../dist/jrcms.dll \
 	JR.Cms.Core.dll JR.Cms.BLL.dll JR.Cms.DAL.dll \
	JR.Cms.Domain.Interface.dll JR.Cms.CacheService.dll \
	JR.Cms.DataTransfer.dll JR.Cms.Domain.Implement.Content.dll \
 	JR.Cms.DB.dll JR.Cms.Cache.dll JR.Cms.Domain.Implement.Site.dll \
 	JR.Cms.Domain.Implement.User.dll JR.Cms.Infrastructure.dll \
 	JR.Cms.Service.dll JR.Cms.ServiceContract.dll \
 	JR.Cms.ServiceUtil.dll JR.Cms.ServiceRepository.dll JR.Cms.IDAL.dll \
 	JR.Cms.Sql.dll JR.Cms.Utility.dll JR.Cms.WebImpl.dll
echo output target: /dist/jrcms.dll

echo "2. prepare files"
tmp_dir=$(pwd)'/dist/tmp'
mkdir -p $tmp_dir/templates && mkdir -p $tmp_dir/bin

cd $(find . -path "*/JR.Cms.WebUI") && \
	cp -r install $tmp_dir &&\
	cp -r plugins $tmp_dir &&\
	cp -r public $tmp_dir &&\
	cp -r templates/default $tmp_dir/templates/ &&\
	cp Global.asax $tmp_dir &&\
	cp Web.config $tmp_dir &&\
        cd - 



echo "2. package upgrade zip"

