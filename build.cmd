@echo off
cls

SET PWD=%CD%\build.fsx

echo %PWD%

cd .paket
paket.bootstrapper.exe
if errorlevel 1 (
  exit /b %errorlevel%
)

paket.exe restore
if errorlevel 1 (
  exit /b %errorlevel%
)

cd ..
packages\FAKE.4.50.0\tools\FAKE.exe "%PWD%" BuildApp
