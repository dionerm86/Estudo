@echo off

call defines.bat


:build
rem goto build_sl

call buildproject.cmd ".\WebGlass.sln"

:build_sl

del "%outputbinaries%\*.pdb

:writePublicKeyToken

echo -------------------------------
echo All done

set Cfg=%1
if %Cfg%_ ==_set Cfg=Pause
if %Cfg%_==/silent_ goto end
pause

:end