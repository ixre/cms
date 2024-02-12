#!/usr/bin/env sh

set -e
echo "======================================="
echo "= JR Cms .NET ! packer ="
echo "======================================="

echo 'Warning: plese make sure build project JR.Cms.AspNet.App first. '

RELEASE_DIR=$(pwd)/out/aspnet

echo "setup1: prepare.." \
    && rm -rf out && mkdir -p ${RELEASE_DIR} \
    && cd src/NetFx/JR.Cms.AspNet.App

if [ $(uname) !=  'Windows' ]; then
  echo "build on others system"
  rm -rf bin
fi

# copy assets from project: jr.cms.app
cp -r ../../JR.Cms.App/install ../../JR.Cms.App/oem  \
   ../../JR.Cms.App/public  ../../JR.Cms.App/templates \
   ../../JR.Cms.App/root .
        
echo "setup2: building.." && \
#xbuild *.csproj /p:Configuration=Release


mkdir ${RELEASE_DIR}/root \
    && cp -r root/*.md ${RELEASE_DIR}/root \
    && mkdir ${RELEASE_DIR}/templates \
    && cp -r templates/default ${RELEASE_DIR}/templates \
    && cp -r public oem install plugins ${RELEASE_DIR} \
    && cp  Global.asax Web.config ${RELEASE_DIR}

# optimize Web.config file
sed -i 's/compilation debug="true"/compilation debug="false"/g' Web.config \
    && sed -i 's/\s*targetFramework="[^"]*"//g' Web.config

# if bin folder not exists. such on linux platform, use prebuild dll files to package. 
if [ -d bin ];then 
    cp -r bin ${RELEASE_DIR}
else
    mkdir ${RELEASE_DIR}/bin \
        && cd ${RELEASE_DIR}/bin \
        && unzip ../../../dll/aspnet_bin.zip >/dev/null
    # clear history version
    rm -rf JR.Cms.dll JR.Stand.* 
    # replace cms core dll
    find ../../../src/JR.Cms.App/bin/Release -name "JR*.dll" | xargs -I {} cp {} .
    # remove net core entrypoint dll
    rm -rf JR.Cms.App.dll
fi

cd ${RELEASE_DIR} \
    && echo "setup3: clean assemblies.." \
    && cd bin && rm -rf *.pdb *.xml *.json *.config roslyn zh-Hans \
    Microsoft.Extensions.DependencyInjection.Abstractions.dll \
    Google.Protobuf.dll Microsoft.DotNet.PlatformAbstractions.dll \
    Microsoft.Extensions.WebEncoders.dll Microsoft.Extensions.Options.dll \
    Microsoft.Extensions.ObjectPool.dll Microsoft.Extensions.Localization* \
    Microsoft.Extensions.FileProviders.Abstractions.dll Microsoft.Extensions.Dependency* \
    Microsoft.Extensions.Configuration.Abstractions.dll Microsoft.Extensions.Caching* \
    Microsoft.Win32.Registry.dll Microsoft.Web.Infrastructure.dll Microsoft.Net.Http.Headers.dll \
    Microsoft.AspNetCore.WebUtilities.dll \
    Microsoft.AspNetCore.JsonPatch.dll Microsoft.AspNetCore.Http.Extensions.dll \
    Microsoft.AspNetCore.Http.dll Microsoft.AspNetCore.Http.Abstractions.dll \
    Microsoft.AspNetCore.Html.Abstractions.dll Microsoft.AspNetCore.Hosting* \
    Microsoft.AspNetCore.A* Microsoft.AspNetCore.C* Microsoft.AspNetCore.D* \
    Microsoft.AspNetCore.Mvc* Microsoft.AspNetCore.M* Microsoft.AspNetCore.R* 

echo 'setup4: upgrade dll..' \
    &&  cp ../../../dll/aspnet/* . \
    &&  cp ../../../dll/fx/* . \
    &&  cd ..
    
echo 'setup5: packing..' \
    && cp ../../LICENSE ../../README.md . \
    && tar czf ../../jrcms-aspnet-latest.tar.gz *
    
echo "package finished!"
 
