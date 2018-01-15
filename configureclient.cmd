@echo off
if "%1" == "" goto configure

:config
set client=%1

:configure

echo ----------------------------
echo Configure Output folder
echo ----------------------------

set outputbinaries=bin/Debug
call buildproject.cmd ".\Clientes\%client%\Glass.Cliente.%client%.Relatorios\Glass.Cliente.%client%.Relatorios.csproj"
xcopy Clientes\%client%\Glass.Cliente.%client%.Relatorios\bin\debug\Glass.Cliente.%client%.Relatorios.dll Glass\bin /R /Y

echo ----------------------------
echo Configured
echo ----------------------------