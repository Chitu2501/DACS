@echo off
echo Creating standalone database updater...
dotnet new console -n DbUpdaterApp -o DbUpdaterTemp
copy DbUpdater.cs DbUpdaterTemp\Program.cs /Y
cd DbUpdaterTemp
dotnet add package System.Data.SqlClient
dotnet build
dotnet run
cd ..
echo Done!
pause 