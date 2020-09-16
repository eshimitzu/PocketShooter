# connects into powershell onto remote runtime of shooter given IP (see ../readme.md)
param([string]$computer,[string]$user = "Administrator")
. $PSScriptRoot/variables.ps1

$command= "cd ${POCKETSHOTER_DEPLOY_ROOT} && pwsh"
Write-Host "Press CTRL+C or type `exit`(no quotes) to leave"
ssh $user@$computer $command