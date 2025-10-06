# Triniti Backend

Triniti Backend is an server that was made to make our recomps able to access online leaderboard, and other stuff.  
Its mainly for Call of Mini games, if you want to host the multiplayer servers, youre in the wrong place.  
**Note**: It will NOT run with any raw decomps, the purpose of this repository is to let people run OUR sources and host their own servers.  

## Supported projects:
|Game|State|
|---------|-----|
|[Call of Mini Dino Hunter](https://github.com/Decomp-And-Recomp/Call-Of-Mini-Dino-Hunter)|Finished|

## Configuration
In order to run the servers, you currently need MySQL database installed. However, SQLite support is planned.

### General
* `hostUrl`: A url used to host the app on.
  * Default: `"http://127.0.0.1:7125"`
* `encryptionKey`: A encryption key used to do the requests. If you want to disable the encryption altogether, make it empty.
  * Default: `"ExampleKey"`
* `enableAntiCheat`: Toggle the Anti Cheat.
  * Default: `false`

### Database
ToDo

### Discord
Discord configuration allows you to use your Discord bot to control the servers.
ToDo

### Dino Hunter
Call of Mini Dino Hunter configuration.
* `maxLeaderboardReturnAmount`: Allows you to set the amount of leaderboard entries the server sends to the client when requested.
  * Default: `200`
