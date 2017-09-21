@echo off

nuget pack MSBuild.Sdk.Extras\MSBuild.Sdk.Extras.nuspec -OutputDirectory Packages -Version %1 -NoPackageAnalysis
