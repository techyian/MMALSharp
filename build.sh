#!/bin/bash
pwd=`pwd`
echo $pwd

.paket/paket.bootstrapper.exe
.paket/paket.exe restore

mono packages/FAKE.4.50.0/tools/FAKE.exe $pwd/build.fsx BuildApp
