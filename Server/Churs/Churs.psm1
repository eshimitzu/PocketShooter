# TODO: as soon as module became big, split out non PocketShooter related part into serparate module Grin (may publish it onto PSGallery as agnostic)
Import-Module -Name powershell-yaml

$POCKETSHOTER_ENVIRONMENT = @{
  ENVIRONMENT = $env:ASPNETCORE_ENVIRONMENT
  DEPLOY_ROOT = "C:/PocketShooter/Server/"
  NAME = "PocketShooter"
  REALTIME_SERVICE = "Photon Socket Server: PocketShooter"
  DATABASE_SERVICE = "PocketShooterDatabase"
  META_SERVICE = "PocketShooterMeta"
  PHOTON_VERSION = "photon-server-sdk_v4-0-29-11263"
  MONGO_VERSION = "mongodb-win32-x86_64-2008plus-ssl-4.0.8"
  RUNTIME = "$($PSScriptRoot)/.runtime/"
  DEPLOY_USER = "Administrator"
  REALTIME_PROCESS = "PhotonSocketServer"
  META_PROCESS = "Heyworks.PocketShooter.Meta.Server.Service"
}

function Start-RealtimeService{
    param(
        [string] $serviceName = $POCKETSHOTER_ENVIRONMENT.REALTIME_SERVICE
    )
    Get-Service -Name $serviceName | Start-Service;
}

function Start-MetaService{
    param(
        [string] $serviceName = $POCKETSHOTER_ENVIRONMENT.META_SERVICE
    )
    Get-Service -Name $serviceName | Start-Service;
}

function Install-Meta{
    param(
      [string] $POCKETSHOTER_RUNTIME
    )  
  if (Get-Service -Name $POCKETSHOTER_ENVIRONMENT.META_SERVICE -ErrorAction SilentlyContinue)
  {
      Write-Host "$($POCKETSHOTER_ENVIRONMENT.META_SERVICE) is installed. Will do nothing"
  }
  else 
  {
      Write-Host "Will do installation of $($POCKETSHOTER_ENVIRONMENT.META_SERVICE)."   
      $path = "$($POCKETSHOTER_RUNTIME)/meta/$($POCKETSHOTER_ENVIRONMENT.META_PROCESS).exe"
      # consider Automatic start stop
      # in case of deploy in next folder
      # register service from here
      # stop old service 
      # from new folder
      New-Service -Name $POCKETSHOTER_ENVIRONMENT.META_SERVICE -BinaryPathName $path -Description "Public front server of PocketShooter" -DisplayName "Pocket Shooter: Meta" -StartupType Automatic
  }
}

function Install-Realtime{
  param(
    [string] $POCKETSHOTER_RUNTIME
   )  
    # install service if not installed
    # we need service because process started via remote is killed when disconnect of remote connection
    if (Get-Service -Name $POCKETSHOTER_ENVIRONMENT.REALTIME_SERVICE -ErrorAction SilentlyContinue)
    {
        Write-Host "$($POCKETSHOTER_ENVIRONMENT.REALTIME_SERVICE) is installed. "
    }
    else 
    {
        Write-Host "Will do installation of $($POCKETSHOTER_ENVIRONMENT.REALTIME_SERVICE)."    
        # puts Computer\HKEY_LOCAL_MACHINE\SYSTEM\ControlSet001\Services\Photon Socket Server: PocketShooter
        $argumentList =  "/Install PocketShooter /configPath `"..\PocketShooterDeploy\bin`" /config Photon.PocketShooter.config"
        Start-Process -FilePath $POCKETSHOTER_ENVIRONMENT.REALTIME_PROCESS -WorkingDirectory "$($POCKETSHOTER_RUNTIME)/photon/deploy/bin_Win64/" -ArgumentList $argumentList -Wait
        # shows Window while should not - so can use sv.exe and reg.exe to do the stuff or use AutoItX or TestStackWHite or Windows Automation to kill window    
    }

    # ---------------------------
    # Photon Socket Server: PocketShooter
    # ---------------------------
    # Photon Socket Server: PocketShooter - 4.0.28.2962

    # Service installed

    # Instance Name = "PocketShooter"
    # ---------------------------
    # OK   
    # ---------------------------

}


function Extract-Realtime {
  param(
    [string] $POCKETSHOTER_RUNTIME
   )  
  # photon download is behind login, so cannot automate
  # choose any download
  # https://gitlab.com/dzmitry-lahoda/photon-server-sdk/raw/master/photon-server-sdk_v4-0-29-11263.zip
  # https://github.com/dzmitry-lahoda/photon-server-sdk/blob/master/photon-server-sdk_v4-0-29-11263.zip?raw=true
  # https://sourceforge.net/p/photon-server-sdk/code/ci/master/tree/photon-server-sdk_v4-0-29-11263.zip?format=raw
  # https://bitbucket.org/Dzmitry_Lahoda/photon-server-sdk/raw/master/photon-server-sdk_v4-0-29-11263.zip



  $archiveVersion = $POCKETSHOTER_ENVIRONMENT.PHOTON_VERSION
  $download = "https://github.com/dzmitry-lahoda/photon-server-sdk/blob/master/$($archiveVersion).zip?raw=true"
  $runtimeFolder = "photon"
  $pathToExe = "$($POCKETSHOTER_RUNTIME)/$($runtimeFolder)/deploy/bin_Win64/PhotonSocketServer.exe"

  Extract-Runtime $pathToExe $POCKETSHOTER_RUNTIME $runtimeFolder $archiveVersion $download;
}

function Extract-Database {
  param(
    [string] $POCKETSHOTER_RUNTIME 
   ) 
    $archiveVersion = $POCKETSHOTER_ENVIRONMENT.MONGO_VERSION
    $download = "http://fastdl.mongodb.org/win32/$($archiveVersion).zip"
    $runtimeFolder = "mongo"
    $pathToExe = "$($POCKETSHOTER_RUNTIME)/$($runtimeFolder)/$($archiveVersion)/bin/mongod.exe"

    Extract-Runtime $pathToExe $POCKETSHOTER_RUNTIME $runtimeFolder $archiveVersion $download;
}

function Start-DatabaseService{
  param(
    [string] $runtime,
    [string] $serviceName = $POCKETSHOTER_ENVIRONMENT.DATABASE_SERVICE
   )  
       $runtime = "$($runtime)/mongo"
       $mongos = Get-ChildItem -Path $runtime | Sort-Object -Descending -Property $_.Name
       $mongoVersion = $mongos[0]    
       # Given local database exanded, setup its environemnts (create directories setup security setting)
       # service requires full path to data and logs
       $logs = "$($mongoVersion.FullName)/log"
       $data = "$($mongoVersion.FullName)/data"
       New-Item -ItemType Directory -Path $logs -Force
       New-Item -ItemType Directory -Path $data -Force
       
       Get-Service -Name $serviceName | Start-Service;
}

function Start-Shooting {
  param(
    [string] $runtime
	)
	Start-DatabaseService "$($runtime)"
	Start-MetaService
	Start-RealtimeService
}

function Extract-Runtime{
    param(
        [string] $pathToExe,
        [string] $POCKETSHOTER_RUNTIME,
        [string] $runtimeFolder,
        [string] $archiveVersion,
        [string] $download
    )    
    if ((Test-Path $pathToExe) -ne 'True') {
        Write-Host "No executable deployed at $($pathToExe). Will deploy."
      
        if (!(Test-Path "$($POCKETSHOTER_RUNTIME)$($runtimeFolder)/")) {
          New-Item -ItemType Directory -Path "$($POCKETSHOTER_RUNTIME)$($runtimeFolder)/"
        }
        
        # TODO if SSH and cannot show progress
        # https://social.technet.microsoft.com/Forums/en-US/32063c17-8375-4511-86ea-b80cf3f02433/how-to-disable-invokewebrequest-progressstatus-bar-when-testing-cas-services
        $ProgressPreference = 'SilentlyContinue'   
      
        if (!(Test-Path "$($env:TEMP)/$($archiveVersion).zip")) {
            Write-Host "Will download $($download)"
            [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12      
            Invoke-WebRequest -Uri $download -OutFile "$($env:TEMP)/$($archiveVersion).zip"
            # TODO: check CRC/SHA
        }
        
        $unpacked = "$($env:TEMP)/$($archiveVersion).zip"
        $destination = "$($POCKETSHOTER_RUNTIME)$($runtimeFolder)/"
        Write-Host "Will unpack $($unpacked)"
        Expand-ArchiveRobust $unpacked $destination
      }      
}

function Expand-ArchiveRobust {
  param (
    [string] $from,
    [string] $to
  )
  try {
    Expand-Archive -Path $from -DestinationPath $to -Force
  }
  catch {
    Write-Host "https://github.com/PowerShell/Microsoft.PowerShell.Archive/issues/77"
    tar -xkf $from -C $to
  }
}

function Stop-ShootingRealtime {
  param(
    [string] $serviceName = $POCKETSHOTER_ENVIRONMENT.REALTIME_SERVICE,
    [string] $processName = $POCKETSHOTER_ENVIRONMENT.REALTIME_PROCESS
   )  
   Stop-ShootingService $serviceName
   if (Get-Process -Name $processName -ErrorAction SilentlyContinue) {
      Write-Host "Will kill $($processName) process"
      $process = Get-Process -Name $processName
      $process.Kill()
  }
}

function Stop-ShootingDatabase {
  param(
    [string] $runtime,
    [string] $serviceName = $POCKETSHOTER_ENVIRONMENT.DATABASE_SERVICE
   )  
   Stop-ShootingService $serviceName
   $runtime = "$($runtime)/mongo"
   if (Test-Path $runtime)
   {
       $mongos = Get-ChildItem -Path $runtime | Sort-Object -Descending -Property $_.Name
       $mongoVersion = $mongos[0]    
       $pathToDaemon = "$($mongoVersion.FullName)\bin\mongod.exe"
       Write-Host "Will stop $($pathToDaemon) if any"
       $running = Get-Process -Name "mongod" -ErrorAction SilentlyContinue | Where-Object {$_.MainModule.FileName -eq $pathToDaemon}
       if ($running) {
           Write-Host "Will stop $($pathToDaemon)"
           $running.Kill()
       }
   }   
}

function Stop-Shooting
{
  param(
    [string] $runtime
   ) 

  Stop-ShootingRealtime
  Stop-ShootingMeta 
  Stop-ShootingDatabase "$($runtime)/mongo"
}

function Stop-ShootingService{
  param(
    [string] $serviceName
  )
  # TODO: handle running service vs process as
  # You can NOT run this instance, "PocketShooter", as an exe as it is already installed as a service.
  # Next fails if not admin, need to think how to make remote as admin
  # Get-Service | Where-Object {$_.Name -eq "Photon Socket Server: PocketShooter" } | Stop-Service  -Force
  # Get-Service : Service 'CDPUserSvc_1440eb6 (CDPUserSvc_1440eb6)' cannot be queried due to the following error:
  if (Get-Service -Name $serviceName -ErrorAction SilentlyContinue) {
    $service = Get-Service -Name $serviceName
    if ($service) {   
        if ($service.CanStop) {
            Write-Host "Will stop $($serviceName)"
            # $service.Stop()
            Stop-Service $service -ErrorAction Stop
        }
    }    
  }    
}


function Stop-ShootingMeta {
  param(
    [string] $serviceName = $POCKETSHOTER_ENVIRONMENT.META_SERVICE,
    [string] $processName =  $POCKETSHOTER_ENVIRONMENT.META_PROCESS
   )  
    Stop-ShootingService $serviceName
    Write-Host "Getting currently running  $($processName) process"

    Wait-Process -Name $processName -Timeout 60
    Write-Host "Process  $($processName) was stoped"

    ##if (Get-Process -Name $processName -ErrorAction SilentlyContinue) {
      ##  Write-Host "Will kill $($processName) process. ISSUE: ASP.NET Service does not die"
      ##  $process = Get-Process -Name $processName
      ##  $process.Kill()
    ##}
}


# open ports by named rule, if rule is disabled than enables it
# issue: need to handle public port on the interent vs internal service 
# TODO: does not work on ubuntu, find right module for mac/linux for pwsh
# TODO: consider align port with process
# TODO Ensure proper firewall public vs local stuff
# Profiles - domain, public, private, Interface Type - Wireless, remote, local
# Seems outbound ports work with no special config
function Expose-ServicePort{
  param(
  [string]$service,
  [string]$port,
  [string]$protocol
  )

  $inName = "PocketShooter$($service)$($protocol)In"
  $inRule = Get-NetFirewallRule -Name $inName -ErrorAction SilentlyContinue
  if ($inRule){
      Write-Host "Port was open earlier"
      if ($inRule.Enabled -eq "False") {
          Write-Host "Will enable $inName"
          Set-NetFirewallRule -Name $inName  -Enabled true
      }
  }
  else {
      Write-Host "Will open in $port"
      New-NetFirewallRule -Name $inName -DisplayName $inName -Enabled True -Direction Inbound -Protocol $protocol -Action Allow -LocalPort $port
  }

  $outName = "PocketShooter$($service)$($protocol)Out"
  $outRule = Get-NetFirewallRule -Name $outName -ErrorAction SilentlyContinue
  if ($outRule){
      Write-Host "Port was open earlier"
      if ($outRule.Enabled -eq "False") {
          Write-Host "Will enable $outName"
          Set-NetFirewallRule -Name $outName  -Enabled true
      }
  }
  else {
      Write-Host "Will open in $port for out"
      New-NetFirewallRule -Name $outName -DisplayName $outName -Enabled True -Direction Outbound -Protocol $protocol -Action Allow -LocalPort $port
  }
}

function Set-EnvironmentRemote{
    param([string]$computer, $user = $POCKETSHOTER_ENVIRONMENT.DEPLOY_USER, [string]$environment="Development")
    "[System.Environment]::SetEnvironmentVariable(""ASPNETCORE_ENVIRONMENT"", ""${environment}"", [EnvironmentVariableTarget]::Machine ) " | ssh $user@${computer} pwsh
    "[System.Environment]::SetEnvironmentVariable(""ASPNETCORE_ENVIRONMENT"", ""${environment}"", [EnvironmentVariableTarget]::Process ) " | ssh $user@${computer} pwsh
    "[System.Environment]::SetEnvironmentVariable(""ASPNETCORE_ENVIRONMENT"", ""${environment}"", [EnvironmentVariableTarget]::User ) " | ssh $user@${computer} pwsh
}

function Restrict-Remote{
    param(
    [Parameter(Mandatory=$true)]
    [string]$clientIpAddress
    )  
    Set-NetFirewallRule -Name RemoteDesktop-UserMode-In-TCP -RemoteAddress $clientIpAddress
    Set-NetFirewallRule -Name RemoteDesktop-UserMode-In-UDP -RemoteAddress $clientIpAddress
    # for linux https://serverfault.com/questions/406839/only-allow-password-authentication-to-ssh-server-from-internal-network
  }

function Setup-ShootingServerRole{
  param(
    [Parameter(Mandatory=$true)]
    [string]$rootFolder
    )  
  $dotnetcore = "https://download.visualstudio.microsoft.com/download/pr/65aecaf4-6011-4882-831d-c9b90cd5033c/55c3561e8ee2629a5298a0ac828fdf0a/dotnet-runtime-2.2.4-win-x64.exe"
  $dotnetfull = "https://download.microsoft.com/download/6/E/4/6E48E8AB-DC00-419E-9704-06DD46E5F81D/NDP472-KB4054530-x86-x64-AllOS-ENU.exe"
  $git = "https://github.com/git-for-windows/git/releases/download/v2.22.0.windows.1/Git-2.22.0-64-bit.exe"
  $vscpp = "https://download.microsoft.com/download/9/3/F/93FCF1E7-E6A4-478B-96E7-D4B285925B00/vc_redist.x64.exe"
  
  # HTTPS with no progress bar
  [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
  $ProgressPreference = 'SilentlyContinue'   
  
  Write-Host "Will download .NET Full"
  Invoke-WebRequest -Uri $dotnetfull -OutFile "$rootFolder/NDP472-KB4054530-x86-x64-AllOS-ENU.exe"
  Write-Host "Will install .NET Full"
  Start-Process -FilePath $rootFolder/NDP472-KB4054530-x86-x64-AllOS-ENU.exe -ArgumentList " /q /norestart" -Wait
  
  Write-Host "Will download .NET Core"
  Invoke-WebRequest -Uri $dotnetcore -OutFile "$rootFolder/dotnet-runtime-2.2.4-win-x64.exe"
  Write-Host "Will install .NET Core"
  Start-Process -FilePath $rootFolder/dotnet-runtime-2.2.4-win-x64.exe -ArgumentList " /quiet" -Wait
  
  Write-Host "Will download GIT"
  Invoke-WebRequest -Uri $git  -OutFile "$rootFolder/Git-2.22.0-64-bit.exe"
  Write-Host "Will install GIT"
  Start-Process -FilePath $rootFolder/Git-2.22.0-64-bit.exe -ArgumentList " /silent" -Wait
  
  Write-Host "Will download VSC++ Redist"
  Invoke-WebRequest -Uri $vscpp -OutFile "$rootFolder/vc_redist.x64.exe"
  Write-Host "Will install VSC++ Redist"
  Start-Process -FilePath $rootFolder/vc_redist.x64.exe -ArgumentList " /silent" -Wait
  
  Write-Host "Will install Go"
  $url = 'https://dl.google.com/go/go1.12.7.windows-amd64.msi';
  Start-Process -FilePath msiexec -ArgumentList "/i $url /quiet /lv $env:TEMP/SilenInstall.log" -Wait;
  C:\Go\bin\go.exe get -u -v github.com/shadowsocks/go-shadowsocks2;

  # TODO: probably set git and go and dotnet as environment paths because will not get these until agent restart
}

function Expose-RealtimePort {
  param(
    [string] $runtime
   ) 

   $xmlPath = "$runtime/photon/deploy/PocketShooterDeploy/bin/Photon.PocketShooter.config"
   $xpath = "/Configuration/PocketShooter/UDPListeners/UDPListener[1]/@Port"
   $port = (Select-Xml $xpath $xmlPath).Node.Value  
   
   Expose-ServicePort  "PocketShooterRealtime" $port UDP
}


function Expose-MetaPort {
  param(
    [string] $runtime
   )

    # ISSUE: ignores ovverides for ASPNETCORE_ENVIRONMENT but easy to do
    $config = (Get-Content "$runtime/meta/appsettings.json" -Raw) | ConvertFrom-Json
    $metaNetwork = $config.Meta.Back.Endpoint
    $siloToSiloPort = $metaNetwork.SiloPort
    $othersToClusterPort = $metaNetwork.GatewayPort

    Expose-ServicePort "PocketShooterMetaBackInternal" $siloToSiloPort TCP
    Expose-ServicePort "PocketShooterMetaBackPublic" $othersToClusterPort TCP

    # HTTP3 is UDP and 5000 is just build in value, so not very robust
    Expose-ServicePort "PocketShooterMetaFront" 5000 TCP

    Expose-ServicePort "PocketShooterSidecar" 8487 TCP
}

function Expose-DatabasePort {
  param(
    [string] $runtime
   )

  # TODO: read from mongo.cfg in $runtime the port
  Expose-ServicePort "PocketShooterDatabase" 27018 TCP
}

function Clean-Meta {
param(
    [string] $runtime
   ) 
   Write-Host "+++++++++ Start clean Meta"
   Write-Host "runtime dir for meta " $runtime/meta
Remove-Item –path $runtime/meta –recurse  –force -ErrorAction Stop
Write-Host "+++++++++ Stop clean Meta"
}

function Clean-Realtime {
param(
    [string] $runtime
   )
   Write-Host "+++++++++ Start clean Realtime"
Remove-Item –path $runtime/photon/deploy/PocketShooterDeploy/bin –recurse  –force -ErrorAction Stop
Write-Host "+++++++++ Stop clean Realtime"
}




Export-ModuleMember -Function Start-DatabaseService, Stop-ShootingDatabase
Export-ModuleMember -Function Clean-Meta, Clean-Realtime
Export-ModuleMember -Function Install-Meta, Start-MetaService, Stop-ShootingMeta
Export-ModuleMember -Function Install-Realtime, Start-RealtimeService, Stop-ShootingRealtime
Export-ModuleMember -Function Extract-Runtime, Extract-Realtime, Extract-Database
Export-ModuleMember -Function Stop-ShootingService, Start-Shooting
Export-ModuleMember -Function Expose-ServicePort, Expose-RealtimePort, Expose-MetaPort, Expose-DatabasePort
Export-ModuleMember -Function Set-EnvironmentRemote
Export-ModuleMember -Function Restrict-Remote
Export-ModuleMember -Function Setup-ShootingServerRole
Export-ModuleMember -Function Expand-ArchiveRobust
Export-ModuleMember -Function Stop-Shooting
Export-ModuleMember -Variable POCKETSHOTER_ENVIRONMENT