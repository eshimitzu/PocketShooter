#Write-Output $env:GIT_COMMIT

#Set-Location "D:\Projects\PrototypePocketShooter\Client\PocketShooter.Client\Assets\Settings\Resources"
$text = $env:GIT_COMMIT + ' ' + $env:GIT_BRANCH
#Create file and set text data
$text | Set-Content '..\..\Settings\Resources\commithash.txt'