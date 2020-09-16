# Pocket Shooter

[PS Concept Google Drive](https://docs.google.com/document/d/1_-DRWQVDP7L_wD_metQ6UU_L9NRBi9cdl0j9o7ED_1k/)

[PT Shoter Google Drive](https://drive.google.com/drive/u/0/folders/1QstRxKVrM_s9svSCJ7BG2YQ28kLJKQ2E)

[PTS: Концепт шутера Pocket Troops - Google Drive](https://docs.google.com/document/d/1cdBFh7sRgHNb41CxR0KNN7ziAIQmeFFcRHtMnQJzscI/edit#heading=h.dzqe8d6nhmhf)

[Jira](https://heyworks.atlassian.net/secure/RapidBoard.jspa?projectKey=PS)

[Slack Chat](https://heyworks.slack.com/messages/CEPHWRW2X/)

## Builds

[Production Android](https://play.google.com/store/apps/details?id=com.pocket.shooter)

[Testing Android (under work account)](https://play.google.com/apps/testing/com.pocket.shooter.test)

## Game design configuration

0. Install [editor](https://www.altova.com/xmlspy-xml-editor).

1. Get latest [configurations](./Server/.gameconfigs) via [git](https://desktop.github.com/).

2. Open `*pocketshooter.json`. Press  `Grid` view. Edit and save.

3. Increase `Version` (Configuration version. Must be update on each modification. First 2 numbers for game releases, 3rd for game fixes and incompatible changes, 4th for A/B testing and tuning)

3. Push.

4. Wait [`Testing` configuration job](http://192.168.88.172:8080/view/Shooter/job/ShooterConfigDeploy/) to finish.

5. If all is OK then configuration is applied. If fails - logs are written on server. There is no configuration version visible on client device as of now.


## Builds

TODO: A. Churs. Please update links here

[Builds on Windows of Server and for Android](http://192.168.88.172:8080/view/Shooter/)

[Builds on Mac for iOS](http://192.168.88.169:8080/view/Shooter/)

[Jenkins config backup for Windows](https://github.com/Heyworks/JenkinsBackup)

# Security

## [Employers](https://docs.google.com/spreadsheets/u/1/d/1cZLqvBVdgO1hvrMsrQzVYJxP5lLNdGn7aDBinEyezmA/edit#gid=0).

## Production 

### Administrator (root)
- password is only possible from office IP
- by key available from any device

#### Database
- servers are behind one special passphrased key, must be stored secured in copies and not used daily.

### Read Database

- read user is created for server and could login only via key
- key is used to tunnel into database accessed via read database user behind password
- new read access can be added by root(when used with special key)

# Servers

http://127.0.0.1:5000/health/status - `Development`, on each build. 

What realtime servers connected and alive? Can be checked via `<META_IP:PORT>/health/status/attached`.

What is meta deployed and of what version?  Via `<META_IP:PORT>/health/status`

Is database alive? via `<META_IP:PORT>/health`.

[Testing](.\Server\.deployment\testing\topology.yml), deployed daily.

[Staging](.\Server\.deployment\staging\topology.yml), deployed for demos (each 2 weeks or like).

[Production](.\Server\.deployment\production\topology.yml), deployed for demos (each 2 weeks or like).


All servers are listed and can be mounted as file systems in configuration of [VS Code SSH FS](https://marketplace.visualstudio.com/items?itemName=Kelvin.vscode-sshfs) of this workspace.



# Development

[Realtime](realtime.md)

**Run `pwsh test-integration.ps1` to ensure you have not broken main gameflow.**

## Common project dependencies updates

If you update `UniRx` please build it for .NET Standard 2.0 and replace it in `Server/libs`. Same action for any other dependency for common projects.

[Logging](logging.md)


# Other

[Как ходить в отпуск, как парень 2.0 - Google Docs](https://docs.google.com/document/d/16BQVgMwEC80nPTXpEKrSpobmC8BODGxHibuGe2zRxA8/edit)

