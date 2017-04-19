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

packages\netframework\FAKE\tools\FAKE.exe "%PWD%" MMALSharpReleaseApp
packages\netframework\FAKE\tools\FAKE.exe "%PWD%" MMALSharpDebugApp

packages\netframework\FAKE\tools\FAKE.exe "%PWD%" MMALSharpCommonReleaseApp
packages\netframework\FAKE\tools\FAKE.exe "%PWD%" MMALSharpCommonDebugApp

packages\netframework\FAKE\tools\FAKE.exe "%PWD%" MMALSharpFFmpegReleaseApp
packages\netframework\FAKE\tools\FAKE.exe "%PWD%" MMALSharpFFmpegDebugApp

.paket\paket.exe pack output build templatefile src\MMALSharp\paket.template