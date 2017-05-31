#!/bin/bash

die() {
    echo $*
    echo Usage: $0 "<NewRelicAgent.framework folder>" && exit 1
}

FRAMEWORK=${1:-"NewRelicAgent.framework"}
[ $FRAMEWORK -a -x $FRAMEWORK ] || die "NewRelicAgent.framework not found or unwritable: [$FRAMEWORK]"

pushd $FRAMEWORK
rm -rf NewRelicAgent Headers Resources Versions/Current
( cd  Versions && ln -s A Current ) || die "Framework files not found!"
ln -s Versions/Current/Headers Headers
ln -s Versions/Current/Resources Resources
ln -s Versions/Current/NewRelicAgent NewRelicAgent
popd

# clean up old meta files. Unity will recreate them
find $FRAMEWORK -name "*.meta" -delete
rm -f $FRAMEWORK.meta

