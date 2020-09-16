# run from powershell.exe as Administrator

if ($PSVersionTable.PSVersion.Major -eq 6)
{
    throw "Please run this script from powershell.exe";
}

Write-Host "Run as Administrator once on Windows as soon as machine obtained in powershell.exe via RPD";
if (!(Test-Path $env:SystemRoot\System32\OpenSSH\sshd.exe)) {
    Write-Host "Will install SSHD"
    Add-WindowsCapability -Online -Name OpenSSH.Server~~~~0.0.1.0; 
    Set-Service -Name sshd -StartupType 'Automatic'; 
    Restart-Service -Name sshd;
}

Write-Host "Will download and install PowerShell Core"
$pwshUrl = "https://github.com/PowerShell/PowerShell/releases/download/v6.2.1/PowerShell-6.2.1-win-x64.msi";
Start-Process -FilePath "msiexec" -ArgumentList " /i $pwshUrl /quiet ADD_EXPLORER_CONTEXT_MENU_OPENPOWERSHELL=1 ENABLE_PSREMOTING=1 REGISTER_MANIFEST=1 " -Wait; 
New-Item -ItemType SymbolicLink -Path c:\pwsh -Value $env:ProgramFiles\PowerShell\6 -Force;

