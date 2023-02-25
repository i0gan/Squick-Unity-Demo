#! /bin/bash
# if nodejs version > 17, set environment bellow
#export NODE_OPTIONS=--openssl-legacy-provider
npm run build:prod
deploy_path=../../deploy
build_path=$deploy_path/data/www
rm -rf $build_path/admin
mkdir -p $build_path
cp -r ./dist $build_path/admin
