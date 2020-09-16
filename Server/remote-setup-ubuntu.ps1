# connects into powershell onto remote runtime of shooter given IP (see ../readme.md)
param([string]$computer,[string]$user="root")

. $PSScriptRoot/variables.ps1

scp -o UserKnownHostsFile=.deployment/production/.ssh/known_hosts ./setup-ubuntu.ps1 $user@${computer}:~
ssh -o UserKnownHostsFile=.deployment/production/.ssh/known_hosts $user@${computer} pwsh ~/setup-ubuntu.ps1