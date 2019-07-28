IF %1.==. GOTO Debug
IF "%1"=="Debug" GOTO Debug
IF "%1"=="debug" GOTO Debug
IF "%1"=="Release" GOTO Release
IF "%1"=="release" GOTO Release

GOTO End1

:Debug
	nuget pack Farenzena.Lib.Database.EF.csproj -IncludeReferencedProjects -Properties Configuration=Debug -OutputDirectory bin\Nuget\Debug
GOTO End1
:Release
	nuget pack Farenzena.Lib.Database.EF.csproj -IncludeReferencedProjects -Properties Configuration=Release -OutputDirectory bin\Nuget\Release
GOTO End1

:End1