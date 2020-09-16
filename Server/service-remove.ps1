
Invoke-Expression $PSScriptRoot/stop-database.ps1

. $PSScriptRoot/variables.ps1

$services = @($POCKETSHOOTER_REALTIME_SERVICE ,$POCKETSHOOTER_DATABASE_SERVICE, $POCKETSHOOTER_META_SERVICE )

foreach ($serviceName in $services) {
    if (Get-Service -Name $serviceName -ErrorAction SilentlyContinue)
    {
        $service  = Get-Service -Name $serviceName
        if ($service)
        {           
            Write-Host "Will remove $($serviceName)"
            Remove-Service $serviceName
        }    
    }    
}
