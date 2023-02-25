#! /bin/bash
# build script for squick www
# author: i0gan
# date: 2022-11-19

ProjectPath=`pwd`/../../../
BuildPath=$ProjectPath/cache
Version="debug"

build_www_server() {
	cd ${ProjectPath}
	mkdir -p "${BuildPath}/www"
	cd "${BuildPath}/www"
	cmake ${ProjectPath}/src/www/server
	if [ $# -gt 0 ]; then
		# Compile all
		echo "Compile $@"
		make $@ -j $(nproc)
	else
		echo "Compile all"
		make -j $(nproc)
	fi
	cd ${ProjectPath}
}

# build
time build_www_server $@

echo "Copying third_paty lib"
cd $project_path
#cp third_party/build/lib/libprotobuf.so ./deploy/bin/lib/libprotobuf.so.32

