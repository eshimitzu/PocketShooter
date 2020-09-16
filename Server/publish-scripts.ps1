param([string]$computer)
. $PSScriptRoot/variables.ps1

Write-Host "Will deploy scripts onto $($computer)"
ssh $POCKETSHOOTER_DEPLOY_USER@${computer} pwsh -Command "New-Item -Path $POCKETSHOTER_DEPLOY_ROOT -ItemType Directory -Force"
scp -rp Churs $POCKETSHOOTER_DEPLOY_USER@${computer}:$POCKETSHOTER_DEPLOY_ROOT
try {
    scp -rp *.ps1 $POCKETSHOOTER_DEPLOY_USER@${computer}:$POCKETSHOTER_DEPLOY_ROOT
}
catch {
    Write-Error "ISSUE: very very very slow as does many connections, instead of one scp session https://github.com/PowerShell/Win32-OpenSSH/issues/988"
    function ScpCopy {
        foreach($fileName in $input) {  scp -rp $fileName $POCKETSHOOTER_DEPLOY_USER@${computer}:$POCKETSHOTER_DEPLOY_ROOT}
    }
    Get-ChildItem -Path $($PSScriptRoot) -File -Filter *.ps1 | ScpCopy
}
