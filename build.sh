#!/bin/bash
pwd=`pwd`
echo $pwd

sudo chmod +x .paket/paket.bootstrapper.exe

.paket/paket.bootstrapper.exe

sudo chmod +x .paket/paket.exe

.paket/paket.exe update

mono packages/FAKE/tools/FAKE.exe $pwd/build.fsx BuildApp
