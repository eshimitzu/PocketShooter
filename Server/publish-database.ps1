param(
[string]$computer
)

. $PSScriptRoot/variables.ps1

Write-Host "Will upload Database configuration onto $computer"
scp -rp .publish/database/mongod.cfg $POCKETSHOOTER_DEPLOY_USER@${computer}:${POCKETSHOTER_DEPLOY_ROOT}.runtime/mongo/
"Import-Module $POCKETSHOTER_DEPLOY_ROOT/Churs; Expose-DatabasePort $POCKETSHOTER_DEPLOY_ROOT/.runtime/;" | ssh $POCKETSHOOTER_DEPLOY_USER@${computer} pwsh 
