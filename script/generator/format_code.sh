#!/usr/bin/env bash

echo "[2] 格式化代码..."
cd generated_code

# 格式化go代码
find ./ -name "*.go*"|xargs sed -i 's/int(.\{1,4\})/int/g'
find ./ -name "*.go*"|xargs sed -i 's/varchar(.\{1,4\})/string/g'
find ./ -name "*.go*"|xargs sed -i 's/decimal(.\{1,4\})/float64/g'

# 格式化kotlin代码
find ./ -name "*.kt"|xargs sed -i 's/int(.\{1,\})/Int/g'
find ./ -name "*.kt"|xargs sed -i 's/varchar(.\{1,\})/String/g'

# 格式化java代码
find ./ -name "*.java"|xargs sed -i 's/int(.\{1,4\})/int/g'
find ./ -name "*.java"|xargs sed -i 's/varchar(.\{1,4\})/String/g'

# 格式化thrift代码
find ./ -name "*.thrift"|xargs sed -i 's/int(.\{1,\})/i32/g'
find ./ -name "*.thrift"|xargs sed -i 's/varchar(.\{1,\})/string/g'
