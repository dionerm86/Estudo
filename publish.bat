@echo off
call defines.bat

if "%1" == "" goto publish

:config
set client=%1

:publish

if exist "%publishdirectory%" (rmdir /S /Q "%publishdirectory%")
mkdir "%publishdirectory%"

set sitedirectory=%cd%\glass

echo ----------------------
echo Clear Glass Bin files
echo ----------------------
del %sitedirectory%\bin\*.*   /s /f  /q > nul

if "%client%" == "" goto build

call configureclient.cmd %client%

:build

call buildproject.cmd ".\WebGlass.sln"

echo Compress site files...
%compress% a "%publishdirectory%\Glass.7z" "%sitedirectory%\*" -r -x!*.cs -x!*.pdb -x!Web.config -x!Upload -x!obj -x!*.csproj -x!*.user > nul


:end