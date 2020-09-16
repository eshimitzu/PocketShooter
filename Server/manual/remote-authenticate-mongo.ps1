param([string]$computer,[string]$user = "root")




# how to do same via PWSH?
#$installKeys = "pwsh -Command  ""& {echo ""$(Get-Content $PSScriptRoot/mongod.security.conf)"" | Out-File -Append -Path /etc/mongod.conf}"""
$installKeys = "cat >> /etc/mongod.conf" 
Get-Content $PSScriptRoot/mongod.security.conf | ssh ${user}@${computer} $installKeys

#sudo chown mongodb /etc/rs.pem