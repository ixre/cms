#!/usr/bin/env sh


rm -rf tmp
mkdir tmp

cp -r ../src/NetFx/JR.Cms.AspNet.App/bin/* ./tmp

cd tmp

rm -rf *.xml *.pdb *.config roslyn

#zip -p -r ../aspnet_bin.zip *

