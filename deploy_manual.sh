#!/usr/bin/env sh

echo '自动镜像构建脚本'

docker=$($(sh -c '! type podman > /dev/null') && echo "docker" || echo "podman")


$docker build -t jarry6/cms:latest -f ./Dockerfile .
$docker push jarry6/cms:latest
