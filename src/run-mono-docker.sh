#!/usr/bin/env sh

# https://stackoverflow.com/questions/42509313/the-default-xml-namespace-of-the-project-must-be-the-msbuild-xml-namespace
docker rm -f mono-cms
docker run --name mono-cms \
    -v $(pwd):/src \
    mono:4.8 sh -c 'ls -al src;cd src/NetFx/JR.Cms.AspNet.App;uname;dotnet xbuild *.csproj /p:Configuration=Release'