# Takes configuration input and publishes to server. SSH keys should be setup beforehand

param([string]$computer)

. $PSScriptRoot/variables.ps1

Write-Host "Will deploy game configurations"
scp -rp $PSScriptRoot/.gameconfigs $POCKETSHOOTER_DEPLOY_USER@${computer}:${POCKETSHOTER_DEPLOY_ROOT}.runtime/meta/
Write-Host "Game configs go into server. Please check logs for issues and database for current state"