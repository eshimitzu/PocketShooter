


- no physics simulation on server
- sever and client maintain common world model(logic only) updated by commands from client
- physics calculations will happen only on client and passed as commands to server
- maximal amount of logic moved to server, but constrained by physics
- example: clip and shots count, damage (dependant on current player parameters) will realized as part of model
- example: who was under attack will came from client (physical world location), but than game snapshots model will work
- compromise solution which allows some cheating
- server may check using some indirect attributes of world if attack was possible, but not in all cases (sometimes full client trust should be used)
- Implementation details and edge cases of Realtime simulation in Timers and Buffers https://docs.google.com/document/d/1DqXlZiNke4L-_aqSaDbDlx4FDYdUMmkfgMIYzNdDISw/edit
- More info about game networking https://github.com/MFatihMAR/Awesome-Game-Networking