
# sets production root key and read keys
param([string]$computer,[string]$user = "root",[string]$identity_file="~/.ssh/id_rsa")
Write-Host "Will setup keys of relevant persons onto remote machine to login no password. Please do some manual steps either"

ssh -i $identity_file -o UserKnownHostsFile=.deployment/production/ssh/known_hosts ${user}@${computer} rm ~/.ssh/authorized_keys
ssh -i $identity_file -o UserKnownHostsFile=.deployment/production/ssh/known_hosts ${user}@${computer} rm /home/read/.ssh/authorized_keys
ssh -i $identity_file -o UserKnownHostsFile=.deployment/production/ssh/known_hosts ${user}@${computer} adduser read

$installRootKey = "umask 077 && mkdir -p ~/.ssh && cat >> ~/.ssh/authorized_keys && echo 'PubkeyAuthentication yes' | cat >> /etc/ssh/sshd_config && sudo service ssh restart "
Get-Content .deployment/production/database/ssh/root/authorized_keys | 
ssh -i $identity_file -o UserKnownHostsFile=.deployment/production/ssh/known_hosts ${user}@${computer} $installRootKey

$installReadKeys = "umask 077 && mkdir -p /home/read/.ssh && cat >> /home/read/.ssh/authorized_keys && chmod 400 /home/read/.ssh/authorized_keys && chown read /home/read/.ssh/authorized_keys  && sudo service ssh restart"
Get-Content .deployment/production/database/ssh/read/authorized_keys | 
ssh -i $identity_file -o UserKnownHostsFile=.deployment/production/ssh/known_hosts ${user}@${computer} $installReadKeys

Write-Host "Will install pwsh"
$installPowerShell = "wget -q https://packages.microsoft.com/config/ubuntu/18.04/packages-microsoft-prod.deb && sudo dpkg -i packages-microsoft-prod.deb && sudo apt-get update && sudo add-apt-repository universe && sudo apt-get install -y powershell && pwsh -Command {Set-ExecutionPolicy -ExecutionPolicy Unrestricted -Scope LocalMachine -Force}"
ssh -i $identity_file -o UserKnownHostsFile=.deployment/production/ssh/known_hosts ${user}@${computer} $installPowerShell


