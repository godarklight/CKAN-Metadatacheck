#!/bin/sh
KSP_VERSION="1.0.5"

if [ ! -d "downloads" ]; then
  mkdir downloads
fi

if [ ! -d "KSP" ]; then
  mkdir KSP
fi

if [ ! -d "KSP/KSP_linux_$KSP_VERSION" ]; then
  echo "Please place a fresh KSP in KSP/KSP_linux_$KSP_VERSION. Press any key to exit"
  read line
  exit
fi

update() {
  clean
  rm -rf KSP/KSP_linux_current/CKAN
  mono ckan.exe ksp forget metacheck
  mono ckan.exe ksp add metacheck KSP/KSP_linux_current
  mono ckan.exe ksp default metacheck
  mono ckan.exe update
  rm -rf temp
  mkdir temp
  cp KSP/KSP_linux_current/CKAN/registry.json temp/
  cp KSP/KSP_linux_current/CKAN/installed-default.ckan temp/
}

clean()
{
  rsync -av --delete KSP/KSP_linux_$KSP_VERSION/ KSP/KSP_linux_current/
  mkdir KSP/KSP_linux_current/CKAN
  ln -s `pwd`/downloads/ KSP/KSP_linux_current/CKAN/downloads
  cp temp/registry.json KSP/KSP_linux_current/CKAN/
  cp temp/installed-default.ckan KSP/KSP_linux_current/CKAN/
}

if [ "$1" = "update" ]; then
  update
fi

if [ "$1" = "clean" ]; then
  clean
fi

if [ "$1" = "" ]; then
  update
  mono CKAN-Metacheck.exe $KSP_VERSION
fi