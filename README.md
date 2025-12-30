# Triniti Backend

Triniti Backend is an server that was made to make our recomps able to access online leaderboard, and other stuff.  
Its mainly for Call of Mini games, if you want to host the multiplayer servers, youre in the wrong place.  
**Note**: It will NOT run with any raw decomps, the purpose of this repository is to let people run OUR sources and host their own servers.  

## Supported projects:
|Game|State|
|---------|-----|
|[Call of Mini Dino Hunter](https://github.com/Decomp-And-Recomp/Call-Of-Mini-Dino-Hunter)|Finished|

## Configuration
In order to run the servers, you currently need MySQL database installed.

### General
|name|description|default|acceptable values|
|----|-----------|-------|-----------------|
|`hostUrl`|A url used to host the app on.|`"http://127.0.0.1:7125"`|url+port (string)|
|`encryptionKey`|A encryption key used to do the requests. If you want to disable the encryption altogether, make it empty.|`"ExampleKey"`|string|
|`enableAntiCheat`|A encryption key used to do the requests. If you want to disable the encryption altogether, make it empty.|`"false"`|true/false|

### Database
|name|description|default|acceptable values|
|----|-----------|-------|-----------------|
|`type`|A database used.|`"MySQL"`|`"MySQL"`|
|`server`|A server where the database is hosted on.|`"127.0.0.1"`|ip (string)|
|`port`|A port where the database is hosted on.|`3306`|integer|
|`userId`|A user id to access the database.|`"root"`|string|
|`password`|A password to access the database.|`""`|string|
|`databaseName`|A database name to use.|`"Triniti"`|string|

### Discord
Discord configuration allows you to use your Discord bot to control the servers.
|name|description|default|acceptable values|
|----|-----------|-------|-----------------|
|`token`|A token of the discord bot.|`""`|string|
|`serverId`|A id of a server where bot will function.|`0`|ulong (long positive number)|
|`loggingChannelId`|A id of a channel where bot will send its logs.|`0`|ulong (long positive number)|
|`allowedRoles`|A list of role ids, that provide access to the bot.|`0`|list of ulong (long positive number)|

### Dino Hunter
|name|description|default|acceptable values|
|----|-----------|-------|-----------------|
|`maxLeaderboardReturnAmount`|Allows you to set the amount of leaderboard entries the server sends to the client when requested.|`200`|integer|

## Credits
[overmet15](https://github.com/overmet15) - Lead Developer.
