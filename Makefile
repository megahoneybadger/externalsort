cleanup:
	dotnet clean core/altium.core.csproj
	dotnet clean repl/sort/altium.repl.sort.csproj
	dotnet clean repl/generator/altium.repl.gen.csproj

restore:
	dotnet restore core/altium.core.csproj
	dotnet restore repl/sort/altium.repl.sort.csproj
	dotnet restore repl/generator/altium.repl.gen.csproj
	
build:
	dotnet build core/altium.core.csproj --configuration Release
	dotnet build repl/sort/altium.repl.sort.csproj --configuration Release
	dotnet build repl/generator/altium.repl.gen.csproj --configuration Release
	
rebuild: cleanup restore build

run: 
	dotnet _bin/altium.gen.dll
