@echo off
cls

SET PWD=%CD%\build.fsx

echo %PWD%

cd .paket
paket.bootstrapper.exe
if errorlevel 1 (
  exit /b %errorlevel%
)

paket.exe update
if errorlevel 1 (
  exit /b %errorlevel%
)

cd ..

packages\FAKE\tools\FAKE.exe "%PWD%" BuildApp
