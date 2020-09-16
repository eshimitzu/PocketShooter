# Runs all all server software on single node(stop any previosly running instances)
# tries to load dll from directory where script is located by default
# cannot use configPath without config, but documented otherwise
# Max length of name of application is 25 symbols
# configPath cannot end on slash
# only \ slashed are allowed, not /
# tried to put into other folder by failed
# have to change current directory to run
# ".runtime/photon/deploy/bin_Win64/PhotonSocketServer.exe" /debug PocketShooter /configPath ".runtime/photon/deploy/PocketShooter/bin" /config Photon.config
param([string]$environment = "Development")
$env:ASPNETCORE_ENVIRONMENT = $environment 

Import-Module $PSScriptRoot/Churs
Stop-Shooting;
$argumentList =  "/debug PocketShooter /configPath `"..\PocketShooter\bin`" /config Photon.PocketShooter.config";
Start-Process -FilePath $POCKETSHOTER_ENVIRONMENT.REALTIME_PROCESS -WorkingDirectory "$PSScriptRoot/.runtime/photon/deploy/bin_Win64/" -ArgumentList $argumentList -WindowStyle Normal;
Write-Host "Press ENTER to stop";
Read-Host;
Stop-Shooting;
# TODO: wait in backgound on on readline for CTRL+C (in that case kill the process and wait it death)