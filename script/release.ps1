param(
    [string]$rootPath
)

msbuild.exe $rootPath\src\echo\Echo.csproj '/consoleLoggerParameters:Summary;Verbosity=minimal' /m /nodeReuse:false /nologo /p:TreatWarningsAsErrors=true /p:Configuration=Sign
