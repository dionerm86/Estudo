if exist %1 goto build
echo Not found %1 - Skipped! 
goto end

:build

echo %configuration%
echo Building %1...
%msbuild% %1 /nologo /verbosity:quiet /t:Build /p:WarningLevel=0 /p:Configuration=%configuration%;Plataform="Any CPU" /p:OutputPath="%outputbinaries%" /p:TreatWarningsAsErrors=true /p:VisualStudioVersion=12.0
echo Done %1

if ERRORLEVEL 1 goto showerror
goto end

:showerror
echo ---------------------------------------------
echo Fail on build
echo ---------------------------------------------

pause

:end