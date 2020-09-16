# connects into powershell onto remote runtime of shooter given IP (see ../readme.md)
param([string]$computer, [string]$user = "Administrator")

# all this could solved by using Windows image with all things preinstalled (Docker, or custom ISO from hardware provider)
. $PSScriptRoot/variables.ps1
ssh $user@${computer} pwsh -Command "New-Item -Path $POCKETSHOTER_DEPLOY_ROOT -ItemType Directory -Force"
ssh $user@${computer} pwsh  -Command { Set-ExecutionPolicy -ExecutionPolicy Unrestricted -Scope LocalMachine -Force}
scp -rp $PSScriptRoot/Churs ${user}@${computer}:${POCKETSHOTER_DEPLOY_ROOT}
"Import-Module $POCKETSHOTER_DEPLOY_ROOT/Churs;Restrict-Remote '86.57.135.135';Setup-ShootingServerRole ${POCKETSHOTER_DEPLOY_ROOT};" | ssh $user@${computer} pwsh 





