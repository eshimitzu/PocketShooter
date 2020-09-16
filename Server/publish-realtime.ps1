param(
    [string]$computer,
    [string]$identity_file = "~/.ssh/id_rsa"
    )

. $PSScriptRoot/variables.ps1

Write-Host "Will upload Realtime onto $computer"
"Import-Module $POCKETSHOTER_DEPLOY_ROOT/Churs; Stop-ShootingRealtime; Clean-Realtime $POCKETSHOTER_DEPLOY_ROOT/.runtime/; Extract-Realtime $POCKETSHOTER_DEPLOY_ROOT/.runtime/; New-Item -Type Directory -Force -Path $POCKETSHOTER_DEPLOY_ROOT/.publish/;" | ssh -i $identity_file $POCKETSHOOTER_DEPLOY_USER@${computer} pwsh 
scp -rp .publish/realtime.zip $POCKETSHOOTER_DEPLOY_USER@${computer}:${POCKETSHOTER_DEPLOY_ROOT}/.publish/
"Import-Module $POCKETSHOTER_DEPLOY_ROOT/Churs; Expand-ArchiveRobust $POCKETSHOTER_DEPLOY_ROOT/.publish/realtime.zip $POCKETSHOTER_DEPLOY_ROOT/.runtime/photon/deploy/; Expose-RealtimePort $POCKETSHOTER_DEPLOY_ROOT/.runtime/;" | ssh -i $identity_file $POCKETSHOOTER_DEPLOY_USER@${computer} pwsh 
