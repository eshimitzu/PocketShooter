# next may requite confirmation
Write-Host "Type A and press Enter to PSGallery as source of packages"
Set-PSRepository PSGallery
if (Get-Module -Name powershell-yaml -ListAvailable) {} else { Install-Module -Name powershell-yaml }
sudo apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv 9DA31620334BD75D9DCB49F368818C72E52529D4
echo "deb [ arch=amd64 ] https://repo.mongodb.org/apt/ubuntu bionic/mongodb-org/4.0 multiverse" | sudo tee /etc/apt/sources.list.d/mongodb-org-4.0.list
sudo apt-get update
sudo apt-get install -y mongodb-org

# performance

ulimit -f unlimited unlimited 

ulimit -t unlimited unlimited 

ulimit -v unlimited unlimited

ulimit -l unlimited unlimited

ulimit -n 64000 64000

ulimit -m unlimited unlimited

ulimit -u 64000 64000
