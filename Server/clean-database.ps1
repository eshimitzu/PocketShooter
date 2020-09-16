

. $PSScriptRoot/variables.ps1

# $ini = ConvertFrom-StringData((Get-Content $PSScriptRoot/.appsettings/mongod.Development.cfg) -join "`n") 
# $argumentList = ($ini.GetEnumerator() | ForEach-Object { if($_.Value -eq "true") { "-$($_.Key)" } elseif ($_.Value -eq  "false") { "" } else {"-$($_.Key)=$($_.Value)" } }) -join " "

$mongoPath = "$($POCKETSHOTER_RUNTIME)mongo/$($MONGO_VERSION)"

if (Test-Path "$($mongoPath)/log") {
    Remove-Item -Path "$($mongoPath)/log" -Recurse -Force
}

New-Item -ItemType Directory -Path "$($mongoPath)/log" -Force

if (Test-Path "$($mongoPath)/data") {
    Remove-Item -Path "$($mongoPath)/data" -Recurse -Force
}

New-Item -ItemType Directory -Path "$($mongoPath)/data" -Force