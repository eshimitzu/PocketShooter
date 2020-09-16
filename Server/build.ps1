param([string]$environment = "Development")

Import-Module -Name powershell-yaml
Import-Module $PSScriptRoot/Churs

$env:ASPNETCORE_ENVIRONMENT = $environment 

Write-Output "Cleaning repository, unpacking runtime, killing running server, building"

Stop-Shooting;

# for developers, start from fresh each time
# TODO: move to rebuild.ps1
#Invoke-Expression $PSScriptRoot/clean-database.ps1

Extract-Database $PSScriptRoot/.runtime/
Extract-Realtime $PSScriptRoot/.runtime/

dotnet build