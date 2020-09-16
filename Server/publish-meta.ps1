param([string]$computer)

. $PSScriptRoot/variables.ps1

Write-Host "Will deploy Meta onto $computer"
"New-Item -Type Directory -Force -Path $POCKETSHOTER_DEPLOY_ROOT/.publish/;" | ssh -i $identity_file $POCKETSHOOTER_DEPLOY_USER@${computer} pwsh 
scp -rp .publish/meta.zip $POCKETSHOOTER_DEPLOY_USER@${computer}:${POCKETSHOTER_DEPLOY_ROOT}/.publish/

"Import-Module $POCKETSHOTER_DEPLOY_ROOT/Churs; Expand-ArchiveRobust $POCKETSHOTER_DEPLOY_ROOT/.publish/meta.zip $POCKETSHOTER_DEPLOY_ROOT/.runtime/; Expose-MetaPort $POCKETSHOTER_DEPLOY_ROOT/.runtime/ ; " | ssh $POCKETSHOOTER_DEPLOY_USER@${computer} pwsh 
