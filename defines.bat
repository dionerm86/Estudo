set dxver=v9.3
set configuration="Debug"
set vsver=vs2013
set outputbinaries=%cd%\bin
set publishdirectory=%cd%\publish

set GACPATH="%WinDir%\assembly\GAC_MSIL\"

set compress="%cd%\7z.exe"

rem Visual Studio 2013 paths
set sn="C:\Program Files\Microsoft SDKs\Windows\v6.0A\Bin\sn.exe" 
set gacutil="C:\Program Files\Microsoft SDKs\Windows\v6.0A\Bin\gacutil.exe"
set msbuild="%WinDir%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe"
set webserver="C:\Program Files (x86)\Common Files\microsoft shared\DevServer\10.0\WebDev.WebServer40.EXE"
set devenv="C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\devenv.exe"

:end