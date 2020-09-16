
# I am

`Developer on Windows` -> `Runtime` -> `Build` -> `Development`, both `All` and `Windows (release starting from 2019)`


# `Runtime`

## All 

- [pwsh v6.2.1](https://github.com/PowerShell/PowerShell/releases/tag/v6.2.1) or later
- `Git` and `Git LFS`. `git` in shell(command line)
- `ssh` and `scp` are in shell(command line). 
- `.NET Core 2.2` runtime or later

## Windows (release starting from 2019)

- [OpenSSH ssh and sshd](https://docs.microsoft.com/en-us/windows-server/administration/openssh/openssh_overview)

- [Git for Windows](https://github.com/git-for-windows/git/releases/download/v2.22.0.windows.1/Git-2.22.0-64-bit.exe)

- [Microsoft Visual C++ Redistributable for Visual Studio 2017 14.16.27027.1](https://download.microsoft.com/download/9/3/F/93FCF1E7-E6A4-478B-96E7-D4B285925B00/vc_redist.x64.exe)

- Only English culture is supported ![Windows English Settings](windows-language-settings.png)

# `Build` (local developer workstations and CI) (after `Runtime`)

## All 

- [.NET Core **SDK v2.2.203**](https://dotnet.microsoft.com/download) 

## Windows (release starting from 2019)

- [net472-developer-pack](https://www.microsoft.com/net/download/thank-you/net472-developer-pack)  for `Realtime.Server.Photon`.

# `Development` (after `Build`)

## All

- `Visual Studio Code 1.33.1` or later, with `C#` and `Powershell` plugin
- [Git Extensions](https://github.com/gitextensions/gitextensions/releases/tag/v3.0.2)

# Automation


Is done via SSH(security, file copy, remote commands) and PWSH(scripting).

## Setup

Run all as Administrator.

# All

`bootstrap*` scrips are first to run, `*authenticate*` then.

Then `setup*.ps1` scripts run on machine. `remote*.ps1` run stuff onto remote.

`*ubuntu*` is for `ubuntu` only.

Then `build*` to build, and than `publish*` to drop on proper deployment onto remotes.

Configuration of build in `*.yml` files.

Please look into scripts details.

We have 2 kins of run - as process for developers and as services for all other environments.