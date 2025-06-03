@echo off
echo Running database update script...
dotnet build CreateLichSuThanhToansTable.cs
dotnet run --project CreateLichSuThanhToansTable.cs
echo Done!
pause 