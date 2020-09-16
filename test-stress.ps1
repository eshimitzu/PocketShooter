param(
[string]$computer
)
. $PSScriptRoot/Server/variables.ps1
$computer = if ($compute) {$compute} else {"127.0.0.1"}
$Env:POCKET_SHOOTER_META = $computer
dotnet test $PSScriptRoot/Client/PocketShooter.Client/Assets/Scripts/Heyworks.PocketShooter.Tests.Integration\Heyworks.PocketShooter.Tests.Integration.csproj --logger "trx;LogFileName=$PSScriptRoot/.tests/TestResults.trx" --results-directory $PSScriptRoot/.tests  --filter Category=Stress --verbosity normal --no-build

