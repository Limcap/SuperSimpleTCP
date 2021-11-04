@echo off

REM PARA GERAR UM PACOTE SO COM O DLL E O PDB
nuget pack SSTcp.Release.nuspec

REM PARA GERAR OS DOIS PACOTES SEPARADOS
REM nuget pack SSTcp.nuspec -Symbols -SymbolPackageFormat snupkg
pause