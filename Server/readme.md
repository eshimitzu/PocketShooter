

**Before starting to work with any of server, ensure you have [installed](setup.md) proper software.**


# Build and run

After [all relevant steps](setup.md) are done, please call `unpack` once. 

Then for each change:
```
build
run
```

Check the logs in `.runtime/photon/deploy/PocketShooter/bin/log/`

# Runtime

- All projects shared with `Unity` and `Meta.Server` are targeted to `netstandard2.0`.
- All projects shared with `Unity` and `Realtime.Server` are targeted to `netstandard2.0`.
- All project shared with `Realtime.Server` and  `Meta.Server` are targeted `netstandard2.0` 
- `Meta.Server` targets `netcoreapp2.2` is portable. 
- `Realtime.Server.Photon` targets `net472` inside updated `Windows 10(with updates starting 2018)` or `Windows Server 2016 (starting with 2017 version)`
- Database is portable.

# Tests
- Open `Server/Heyworks.PocketShooter.Server.Tests.Integration`. 
- Run `dotnet run` in terminal or build in IDE.
- Open another terminal.
  
# Overall picture
See [diagrams](diagrams.md)

# Meta


```feature
Feature: Profile, shop, matchmaking, chat, clans, skills, grades, base, 

  Scenario: Player presses `Battle` button
  Given Player pressed battle button
  Then Matchmaking begins 
  When Match is ready 
  Then Player is routed to MatchServer

Scenario: Authentication
  Given Player gets token from Authenticator (Facebook, Apple, Google), not only OAuth
  When Player passes token into Meta.Server
  Then Player gets PlayerId

Scenario: Payment 
    Given Player money are withdrawn by Market
      And Player gets Check.
    When Player provides Check to Meta.Server
    Then Meta.Server connection is secured
      And Meta.Server validates check by Market
```

# Deployment

[Naming as in Deployment environment](https://en.wikipedia.org/wiki/Deployment_environment).

- deployment are described in `topology.yml`

- meta server deployed on 1 physical instance

- realtime server deployed on several physical instances with photon licenses

- mongo deployed on replica instance (1 master, 2 replica) on unbuntu

- database is ALWAYS ON, do not restart, migrations are no yet done

- monitoring of health urls and monitoring of memory-cpu is no done

- integration test hangs on device register:(

- remote logging is not done

- no mechanism to gracefully stop the game. so any deploy after production run will aborting games yet

- clean folder before new copy is done

- proper deploy archive into .publish/production/version -> drop onto remote -> stop service -> swap run (cleaner and faster and more robust) no done yet

- no all scrips report errors right and some errors are not errors

- move scripts int scripts subfolder to avoid noise and pollution, split scripts accoreding run (all, from local to remote, remote, runtime only, build only)