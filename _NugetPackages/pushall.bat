@echo off
setlocal

REM Değiştirmen gereken yerler:
set NUGET_EXE_PATH=C:\Users\Admin\source\AlpKit\_NugetPackages\nuget.exe
set NUPKG_FOLDER=C:\Users\Admin\source\AlpKit\_NugetPackages
set NUGET_SOURCE=https://pkgs.dev.azure.com/alperbasda/_packaging/alp_kit/nuget/v3/index.json
set API_KEY=CwE62w9vv3OErxYkZoXeogEE6KVIVHu1aKilXum5Ua6vgzCClEfqJQQJ99BDACAAAAAAAAAAAAASAZDO2gRq

echo Starting to push .nupkg files...

for %%f in (%NUPKG_FOLDER%\*.nupkg) do (
    echo Pushing: %%f
    "%NUGET_EXE_PATH%" push "%%f" -Source "%NUGET_SOURCE%" -ApiKey "%API_KEY%"
)

echo Done pushing all packages!
pause
