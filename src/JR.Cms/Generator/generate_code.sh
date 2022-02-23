#!/usr/bin/env bash

# Files like this:
# ```
# + generator
#  - templates
#  - tto.conf
# - example_run.sh
# ```

if [[ $(whereis tto) = 'tto:' ]]; then
  echo '未安装tto客户端,请运行安装命令： curl -L https://raw.githubusercontent.com/ixre/tto/master/install | sh'
fi

CONF_DIR=$(dirname "$0")
tto -conf "$CONF_DIR"/tto.conf -t "$CONF_DIR"/templates -o output -excludes tmp_ -clean

# Replace generator description part of code file
# find output/spring -name "*.java" -print0 |  xargs -0 sed -i ':label;N;s/This.*Copy/Copy/g;b label'
# Replace package
# find output/spring -name "*.java" -print0 |  xargs -0 sed -i 's/net.fze/cn.cgt/g'
# Replace type
# find output/java -name "*.java"  -print0 | xargs -0 sed -i 's/ int / Integer /g'
# copy files to project folder
# find ./src -path "*/entity" -print0 | xargs -0 cp output/spring/src/main/java/com/github/tto/entity/*

exit 0;
