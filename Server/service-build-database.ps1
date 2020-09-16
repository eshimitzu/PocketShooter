param([string]$environment = "Development")
$environment = $environment.tolower();

. $PSScriptRoot/variables.ps1

$publish = "$($PSScriptRoot)/.publish/database"
New-Item -ItemType Directory -Path $publish -Force
Copy-Item -Path "$($PSScriptRoot)/.appsettings/mongod.${environment}.cfg" -Destination $publish
if (Test-Path "${publish}/mongod.cfg") {
    Remove-Item -Path "${publish}/mongod.cfg" -Force
}
Rename-Item -Path "${publish}/mongod.${environment}.cfg" -NewName "mongod.cfg"


