
# run this as Administrator in pwsh on target machine

if ($PSVersionTable.PSVersion.Major -lt 6)
{
    throw "Please run it from pwsh.exe";
}


Write-Host "Based on https://docs.microsoft.com/en-us/windows-server/administration/openssh/openssh_server_configuration";
Write-Host "Grab keys from local ./ssh/id_rsa.pub or from .deployment/ASPNETCORE_ENVIRONMENT/*/authorized_keys";

New-Item -Type Directory ~/.ssh/ -Force;
New-Item -Type File ~/.ssh/authorized_keys -Force;

$whereToPutKeys = "~/.ssh/authorized_keys";
Write-Host "Put it into $whereToPutKeys";
Write-Host "Type ENTER as done";
Read-Host;

$whereIsConfig = "$($env:ProgramData)/ssh/sshd_config";
"PubkeyAuthentication yes" | Out-File -Append $whereIsConfig -Encoding ascii;
"Subsystem    powershell    C:/pwsh/pwsh.exe -sshs -NoLogo -NoProfile" | Out-File -Append $whereIsConfig -Encoding ascii;
(Get-Content $whereIsConfig) | ForEach-Object { $_ -replace 'PasswordAuthentication yes', '#PasswordAuthentication no' } | ForEach-Object { $_ -replace 'Match Group Administrator', '#Match Group Administrator' } | ForEach-Object { $_ -replace 'AuthorizedKeysFile __PROGRAMDATA__/ssh/administrators_authorized_keys', ' #AuthorizedKeysFile __PROGRAMDATA__/ssh/administrators_authorized_keys' } | Out-File $whereIsConfig -Encoding ascii;

Get-Service -Name sshd | Restart-Service -Verbose;

Write-Host "Will install PowerShell Core modules";
Set-ExecutionPolicy -ExecutionPolicy Unrestricted -Scope LocalMachine -Force;
Write-Host "Type A to add PSGallery and press ENTER";
Set-PSRepository PSGallery;
Install-Module -Name powershell-yaml;
Install-Module -Name SilentInstall;
# may consider 
# DockerMsftProvider
# posh-git
# Posh-SSH 
# NetworkingDsc
# WinSSH

#
# I was not able to automte that like or linux.
# How to request password only once? How to drop-append content of keys? How to edit sshd_config? 
# Why so much modifications needed in contrast with linux? How to enasure authorized_keys dir and file created as Administrator so rigt ssh credentials.
# Have tried to pass via pipe, -Commnad {}, via parameters, Out-File Get-Content,
# `0 -- Null
# `a -- Alert
# `b -- Backspace
# `n -- New line
# `r -- Carriage return
# `t -- Horizontal tab
# `' -- Single quote
# `" -- Double quote


