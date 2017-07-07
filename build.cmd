@echo off
cls

dotnet restore
dotnet build --framework net452 --configuration Release
dotnet build --framework netcoreapp1.1 --configuration Release