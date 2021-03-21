@echo off
if exist publish\ rmdir publish\ /s /q
dotnet publish --nologo -c Release -o publish