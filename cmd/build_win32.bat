@echo off
color 66

echo =======================================
echo = J6 Cms .NET ! 核心程序集生成工具 =
echo =======================================

set cur=%cd%
%~d0
cd ..\
echo 上级目录为：%cd%
set dir=%cd%

echo "set dir2=%~dp0 echo 当前目录">nul
set megdir=%dir%\dist\bin\

if exist "%cur%/merge.exe" (

  echo 生成中,请稍等...
  cd %dir%/src/bin/

echo  /keyfile:%dir%\src\J6.Cms.Core\j6.cms.snk>nul

"%cur%/merge.exe" /closed /ndebug /targetplatform:v4 /target:dll /out:%dir%/dist\j6.cms.dll^
 J6.Cms.Core.dll J6.Cms.BLL.dll J6.Cms.DAL.dll J6.Cms.Domain.Interface.dll^
 J6.Cms.CacheService.dll J6.Cms.DataTransfer.dll J6.Cms.Domain.Implement.Content.dll^
 J6.Cms.DB.dll J6.Cms.Cache.dll J6.Cms.Domain.Implement.Site.dll J6.Cms.Domain.Implement.User.dll J6.Cms.Infrastructure.dll ^
 J6.Cms.Service.dll J6.Cms.ServiceContract.dll^
 J6.Cms.ServiceUtil.dll J6.Cms.ServiceRepository.dll J6.Cms.IDAL.dll^
 J6.Cms.Sql.dll J6.Cms.Utility.dll StructureMap.dll J6.Cms.WebImpl.dll


  echo 完成!输出到:%dir%/dist/j6.cms.dll

)


pause