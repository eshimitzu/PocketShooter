

For any operations ensure `powershell-yaml` is installed into `pwsh`.


# Drop scripts

Without this nothing will work. So please ensure module in folder

```powershell
scp -rp ./Server/Churs Administrator@94.130.51.232:C:/PocketShooter/Server/
```

# Remotely restarts all possible shooting services:

```powershell
"Import-Module C:/PocketShooter/Server/Churs; Stop-Shooting; Start-Shooting C:/PocketShooter/Server/.runtime/;" | ssh Administrator@94.130.51.232 pwsh
```

