. $PSScriptRoot/variables.ps1
# TODO: read parameters from topology
Invoke-Expression -Command "$($PSScriptRoot)/service-install-database-ubuntu.ps1 95.211.255.185 database_1 Production"
Invoke-Expression -Command "$($PSScriptRoot)/service-install-database-ubuntu.ps1 81.171.3.89 database_2 Production"
Invoke-Expression -Command "$($PSScriptRoot)/service-install-database-ubuntu.ps1 81.171.3.87 database_3 Production"