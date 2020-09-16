# Takes build output and publishes to server. ssh -i $identity_file keys should be setup beforehand

param(
[string]$computer,
[string]$identity_file = "~/.ssh/id_rsa" 
)
# TODO: use one settion for all, pass key down the stack (use in scp)
# todo: put whole publish as archives into .publish (as for now to slow to publish)
# until https://heyworks.atlassian.net/browse/PSH-904 must restart meta after realtime restart
# document issues with current deploy and propitize after https://heyworks.atlassian.net/browse/PSH-946
# New-PSSession -Port 22 -Host 23.105.40.220 -UserName Administrator -SSHTransport

. $PSScriptRoot/variables.ps1

Import-Module -Name powershell-yaml
Import-Module $PSScriptRoot/Churs 
$deploymentConf = Get-Content "$($PSScriptRoot)/.publish/deployment.yml" -Raw | ConvertFrom-Yaml
$topology = Get-Content "$($PSScriptRoot)/.deployment/$($deploymentConf.environment)/topology.yml" -Raw | ConvertFrom-Yaml

Write-Host $($deploymentConf.environment)

# non production
if ($topology.database.nodes.length -eq 1 -and $deploymentConf.environment -ne 'Production') {
    Write-Host '==== Deploying Database ======'
    foreach ($database in $topology.database.nodes){
            $computer = $database.publicIp

            Invoke-Expression -Command "$($PSScriptRoot)/publish-scripts.ps1 $computer"
            Set-EnvironmentRemote $computer $POCKETSHOOTER_DEPLOY_USER $($deploymentConf.environment)
            "Import-Module $POCKETSHOTER_DEPLOY_ROOT/Churs; Stop-ShootingDatabase;Extract-Database $POCKETSHOTER_DEPLOY_ROOT/.runtime/" | ssh -i $identity_file $POCKETSHOOTER_DEPLOY_USER@${computer} pwsh 
            Invoke-Expression -Command "$($PSScriptRoot)/publish-database.ps1 $computer"
            ssh -i $identity_file $POCKETSHOOTER_DEPLOY_USER@${computer} pwsh $POCKETSHOTER_DEPLOY_ROOT/service-install-database.ps1
            "Import-Module $POCKETSHOTER_DEPLOY_ROOT/Churs; Start-DatabaseService $POCKETSHOTER_DEPLOY_ROOT/.runtime/;" | ssh -i $identity_file $POCKETSHOOTER_DEPLOY_USER@${computer} pwsh            
    }
}
else
{
    Write-Host "==== Production Database is deployed separately ===="
}

Write-Host '==== Deploying Meta ======'
foreach ($node in $topology.meta.nodes){
        $computer = $node.publicIp
        Invoke-Expression -Command "$($PSScriptRoot)/publish-scripts.ps1 $computer"
        Set-EnvironmentRemote $computer $POCKETSHOOTER_DEPLOY_USER $($deploymentConf.environment)
        "Import-Module $POCKETSHOTER_DEPLOY_ROOT/Churs; Stop-ShootingMeta" | ssh -i $identity_file $POCKETSHOOTER_DEPLOY_USER@${computer} pwsh         
        "Import-Module $POCKETSHOTER_DEPLOY_ROOT/Churs; Clean-Meta $POCKETSHOTER_DEPLOY_ROOT/.runtime" | ssh -i $identity_file $POCKETSHOOTER_DEPLOY_USER@${computer} pwsh
        Invoke-Expression -Command "$($PSScriptRoot)/publish-meta.ps1 $computer"
        "Import-Module $POCKETSHOTER_DEPLOY_ROOT/Churs; Install-Meta $POCKETSHOTER_DEPLOY_ROOT/.runtime/; Start-MetaService;" | ssh -i $identity_file $POCKETSHOOTER_DEPLOY_USER@${computer} pwsh 
}

# # ISSUE: if meta is not deployed, it must restarted to avoid dead rooms

Write-Host '==== Deploying Realtime ======'
foreach ($node in $topology.realtime.nodes){
        $computer = $node.publicIp
        Invoke-Expression -Command "$($PSScriptRoot)/publish-scripts.ps1 $computer"
        #Set-EnvironmentRemote $computer $POCKETSHOOTER_DEPLOY_USER $($deploymentConf.environment)
        
        Invoke-Expression -Command "$($PSScriptRoot)/publish-realtime.ps1 $computer $identity_file"

        $environmentDirectory = $deploymentConf.environment.tolower()
        foreach ($file in $topology.realtime.runtime.copy){
            $filePath = "$PSScriptRoot\.deployment\$($environmentDirectory)\$($file.path)"
            scp -rp $filePath $POCKETSHOOTER_DEPLOY_USER@${computer}:${POCKETSHOTER_DEPLOY_ROOT}/.runtime/$($file.into)
        }        
    
        "Import-Module $POCKETSHOTER_DEPLOY_ROOT/Churs; Install-Realtime $POCKETSHOTER_DEPLOY_ROOT/.runtime/; Start-RealtimeService;" | ssh -i $identity_file $POCKETSHOOTER_DEPLOY_USER@${computer} pwsh 
}

Write-Host '==== Restart Meta for testing purpose ======'
"Import-Module $POCKETSHOTER_DEPLOY_ROOT/Churs; Stop-ShootingMeta; Start-MetaService;" | ssh -i $identity_file $POCKETSHOOTER_DEPLOY_USER@${computer} pwsh

Write-Host "All services are hopefully running. Please check http://$($topology.meta.nodes.publicIp):5000/health/status and http://$($topology.meta.nodes.publicIp):5000/health/status/attached"