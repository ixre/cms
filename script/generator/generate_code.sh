#!/usr/bin/env bash

arch=""
tables=""
conf="./"
tpl="./templates"
out="./output"

while getopts "a:c:t:v" args
do
  case ${args} in
      a)
        arch=${OPTARG};;
      c)
        conf=${OPTARG};;
      t)
        tables=${OPTARG};;
      v)
        gof-gen -v && exit 1;;
      \?)
        echo "Usage: args [-a] [-t] [-v]"
        echo " -a : code arch"
        echo " -c : config path"
        echo " -t : table prefix"
        echo " -v : print version"
        exit 1
  esac
done


params="-tpl ${tpl} -out ${out}"

if [[ ${conf} != "" ]];then
    params="${params} -conf ${conf}"
fi
if [[ "${arch}" != "" ]];then
    params="${params} -arch ${arch}"
fi
if [[ "${tables}" != "" ]];then
    params="${params} -table ${tables}"
fi

../gof-gen ${params}
