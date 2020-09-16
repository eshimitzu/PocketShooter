. $PSScriptRoot/variables.ps1
Import-Module -Name powershell-yaml
$mongoPath = "$($POCKETSHOTER_RUNTIME)mongo/$($MONGO_VERSION)"
$mongoConf = Get-Content "$($POCKETSHOTER_RUNTIME)mongo/mongod.cfg" -Raw | ConvertFrom-Yaml
$mongoConf.systemLog.path = "$($mongoPath)/log/PocketShooter.log"
$mongoConf.storage.dbPath = "$($mongoPath)/data"    
$mongoConf | ConvertTo-Yaml | Out-File -Path $mongoPath/mongod.cfg

if (Get-Service -Name $POCKETSHOOTER_DATABASE_SERVICE -ErrorAction SilentlyContinue) {
    Write-Host "$($POCKETSHOOTER_DATABASE_SERVICE) is installed. Will do nothing"
}
elseif ($mongoConf) {
    Write-Host "Will do installation of $($POCKETSHOOTER_DATABASE_SERVICE)."

    # sometimes, also 
    #2019-08-09T09:26:15.507+0000 F CONTROL  [main] Failed global initialization: FileNotOpen: Failed to open "C:\PocketShooter\Server/.runtime/mongo/mongodb-win32-x86_64-2008plus-ssl-4.0.8/log/PocketShooter.log"
    New-Item -Type Directory -Force -Path "$($mongoPath)/log/";
    New-Item -Type Directory -Force -Path "$($mongoPath)/data/";
    $argumentList = "--install --serviceName $($POCKETSHOOTER_DATABASE_SERVICE) --serviceDisplayName ""PocketShooter: Database"" --serviceDescription """" --config $($mongoPath)/mongod.cfg"
    Start-Process -FilePath "$($mongoPath)/bin/mongod.exe" -WorkingDirectory $mongoPath -NoNewWindow -ArgumentList $argumentList -Wait
}
else {
    Write-Error -Message "Failed to install database"
}




