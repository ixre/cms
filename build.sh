#!/usr/bin/env sh

echo "======================================="
echo "= JR Cms .NET ! packer ="
echo "======================================="

RELEASE_DIR=$(pwd)/out/release

rm -rf out && mkdir -p ${RELEASE_DIR} && \
    cd src/JR.Cms.App && dotnet restore && \
    dotnet publish -c Release -o ${RELEASE_DIR} && \
    cp -r public templates oem install plugins root ${RELEASE_DIR} && \
    cd ${RELEASE_DIR} && \
    rm -rf *.pdb *.xml appsettings.json appsettings.Development.json && \
    cp ../../LICENSE ../../README.md . && \
    tar czf ../../jrcms-latest.tar.gz *
    
echo "package finished!"
    

    