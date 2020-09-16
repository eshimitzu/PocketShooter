# Builds all server software in Release configuration
param([string]$environment = "Development")



# TODO: Move to churs into Clean-Build

dotnet clean
if (Test-Path $PSScriptRoot/.publish/) {
    Remove-Item -Path $PSScriptRoot/.publish/ -Force -Recurse
}
New-Item -ItemType Directory -Path $PSScriptRoot/.publish/ -Force

$deploymentDescription = @{
    environment = $environment
}

#TODO: Fail build if yaml module does not installed

Import-Module -Name powershell-yaml
Import-Module $PSScriptRoot/Churs

# need to set to pass down to dotnet build conditionals
$env:ASPNETCORE_ENVIRONMENT = $environment 



$deploymentDescription | ConvertTo-Yaml | Out-File -Path $PSScriptRoot/.publish/deployment.yml

Write-Host "Will build for $($environment) environment";
Write-Output "Cleaning repository, unpacking runtime, killing running server, building";
Extract-Realtime "$PSScriptRoot/.runtime/" ;
Extract-Database "$PSScriptRoot/.runtime/" ;

Invoke-Expression "$PSScriptRoot/service-build-database.ps1 $environment"
Stop-Shooting;
dotnet build -c Release

$meta = "./Heyworks.PocketShooter.Meta.Server.Service/Heyworks.PocketShooter.Meta.Server.Service.csproj"
dotnet publish $meta --output $PSScriptRoot/.runtime/meta/ --configuration Release
Compress-Archive -Path $PSScriptRoot/.runtime/meta/ -DestinationPath $PSScriptRoot/.publish/meta.zip -Force

Compress-Archive -Path $PSScriptRoot/.runtime/photon/deploy/PocketShooterDeploy/ -DestinationPath $PSScriptRoot/.publish/realtime.zip -Force


