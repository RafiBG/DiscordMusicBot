# Description
Made with C# on .NET 9.0 and libraries: DSharpPlus, DsharpPlus.CommandsNext, DsharpPlus.Interactivity, 
DsharpPlus.Lavalink, Newtonsoft.Json, DSharpPlus.SlashCommands
# How to use it
In folder Config there is config.json add your Discord token then copy the whole file config.json and go to
MusicBotDiscord\MusicBotDiscord\bin\Debug\net9.0 and paste it there also there is your MusicBotDiscord.exe
to start the program. <br /> <br />
If the you get in the console this error: [Crit ] Failed to connect to Lavalink change the config.json server or create one. <br /> Link: https://lavalink.darrennathanael.com/SSL/lavalink-with-ssl/#tunnelbroker-guide
# New slash commands
/play (name or YouTube link) <br />
/pause <br />
/resume <br />
/stop - it will leave the voice chat  <br />
/volume (number)
# Old commands (they still work just not active)
!help - Shows all the commands <br />
!play (name or YouTube link) <br />
!pause <br />
!resume <br />
!stop - it will leave the voice chat <br />
!volume (number)

To make the old commands work uncomment this line in Program.cs  //Commands.RegisterCommands<MusicCommands>(); //register commands with prefix symbol  ! 

