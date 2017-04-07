@echo off
cls

SET ROOT=%CD%
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

packages\FAKE\tools\FAKE.exe "%PWD%" Clean
packages\FAKE\tools\FAKE.exe "%PWD%" MMALSharpReleaseApp
packages\FAKE\tools\FAKE.exe "%PWD%" MMALSharpDebugApp

.paket\paket.exe pack output build templatefile MMALSharp\paket.template