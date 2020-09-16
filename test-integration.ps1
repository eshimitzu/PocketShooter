# runs integration tests agains specified meta
# `pwsh test-integration.ps1 94.130.51.232`

param(
[string]$computer
)

. $PSScriptRoot/Server/variables.ps1

$computer = if ($compute) {$compute} else {"127.0.0.1"}

$Env:POCKET_SHOOTER_META = $computer


dotnet test $PSScriptRoot/Client/PocketShooter.Client/Assets/Scripts/Heyworks.PocketShooter.Tests.Integration\Heyworks.PocketShooter.Tests.Integration.csproj --logger "trx;LogFileName=$PSScriptRoot/.tests/TestResults.trx" --results-directory $PSScriptRoot/.tests --filter Category=Integration --verbosity normal 

#--no-build

dotnet publish .\Client\PocketShooter.Client\Assets\Scripts\Heyworks.PocketShooter.Console.Runner\Heyworks.PocketShooter.Console.Runner.csproj -c Release 
dotnet exec .\Client\PocketShooter.Client\Assets\Scripts\Heyworks.PocketShooter.Console.Runner\.bin\Release\netcoreapp2.2\publish\Heyworks.PocketShooter.Console.Runner.dll  --meta 23.105.40.220 --ticks 500 --players 40 --cycles 2