@echo off
MSBuild Limcap.SSTcp.csproj /t:Rebuild /p:Configuration=Debug
MSBuild Limcap.SSTcp.csproj /t:Rebuild /p:Configuration=Release