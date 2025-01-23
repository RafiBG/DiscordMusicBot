using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Lavalink;

namespace MusicBotDiscord.Commands
{
    public class MusicCommands : BaseCommandModule
    {
        
        [Command("play")]
        public async Task PlayMusic(CommandContext ctx, [RemainingText] string query)
        {
            // Why var userVC = ctx.Member?.VoiceState?.Channel; has this symbol ?
            // well if you are NOT in a voice channel it will crash
            // and trow null but this way with symbol ? will have null and continue
            // to the if where it will be done
            var userVC = ctx.Member?.VoiceState?.Channel;
            var lavalinkInstance = ctx.Client.GetLavalink();

            //check if user is in valid channel
            if (ctx.Member.VoiceState == null || userVC == null)
            {
                await ctx.Channel.SendMessageAsync("Please join in valid voice channel first");
                return;
            }

            if (!lavalinkInstance.ConnectedNodes.Any())
            {
                await ctx.Channel.SendMessageAsync("Connection is not established");
                return;
            }

            if (userVC.Type != DSharpPlus.ChannelType.Voice)
            {
                await ctx.Channel.SendMessageAsync("Please enter a valid voice channel");
                return;
            }

            var node = lavalinkInstance.ConnectedNodes.Values.First();
            await node.ConnectAsync(userVC);

            var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);
            if (conn == null)
            {
                await ctx.Channel.SendMessageAsync("Lavalink failed to connect");
                return;
            }

            var searchQuery = await node.Rest.GetTracksAsync(query);
            if (searchQuery.LoadResultType == LavalinkLoadResultType.NoMatches || searchQuery.LoadResultType == LavalinkLoadResultType.LoadFailed)
            {
                await ctx.Channel.SendMessageAsync($"Failed to find music with query: {query}");
                return;
            }

            var musicTrack = searchQuery.Tracks.First();

            await conn.PlayAsync(musicTrack);

            string musicDescription = $"Now playing: {musicTrack.Title} \n" +
                                      $"Author: {musicTrack.Author} \n" +
                                      $"Video lenght: {musicTrack.Length} \n" +
                                      $"URL: {musicTrack.Uri}";

            var nowPlayingEmbed = new DiscordEmbedBuilder()
            {
                Color = DiscordColor.Blue,
                Title = $"Succesfully joined channel {userVC.Name} and playing music",
                Description = musicDescription
            };

            await ctx.Channel.SendMessageAsync(embed: nowPlayingEmbed);

            var NoUsersInVCLeave = new DiscordEmbedBuilder()
            {
                Color = DiscordColor.Red,
                Title = "There are no users in the voice channel. Leaving...",
            };

            // if there are no users in the voice channel the Bot will leave
            while (true)
            {
                var Users = userVC.Users;
                int UsersCount = Users.Count();
                if (UsersCount == 1) 
                {
                    await conn.StopAsync();
                    await conn.DisconnectAsync();

                    await ctx.Channel.SendMessageAsync(embed: NoUsersInVCLeave);
                    return;
                }
                await Task.Delay(10000);
            }
        }

        [Command("pause")]
        public async Task PauseMusic(CommandContext ctx)
        {
            var userVC = ctx.Member?.VoiceState?.Channel;
            var lavalinkInstance = ctx.Client.GetLavalink();

            //check if user is in valid channel
            if (ctx.Member.VoiceState == null || userVC == null)
            {
                await ctx.Channel.SendMessageAsync("Please join in valid voice channel first");
                return;
            }

            if (!lavalinkInstance.ConnectedNodes.Any())
            {
                await ctx.Channel.SendMessageAsync("Connection is not established");
                return;
            }

            if (userVC.Type != DSharpPlus.ChannelType.Voice)
            {
                await ctx.Channel.SendMessageAsync("Please enter a valid voice channel");
                return;
            }

            var node = lavalinkInstance.ConnectedNodes.Values.First();
            var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

            if (conn == null)
            {
                await ctx.Channel.SendMessageAsync("Lava link failed to connect");
                return;
            }

            if (conn.CurrentState.CurrentTrack == null)
            {
                await ctx.Channel.SendMessageAsync("No tracks are playing");
                return;
            }

            await conn.PauseAsync();

            var pausedEmbed = new DiscordEmbedBuilder()
            {
                Color = DiscordColor.Yellow,
                Title = "Track paused"
            };

            await ctx.Channel.SendMessageAsync(embed: pausedEmbed);

        }

        [Command("resume")]
        public async Task ResumeMusic(CommandContext ctx)
        {
            var userVC = ctx.Member?.VoiceState?.Channel;
            var lavalinkInstance = ctx.Client.GetLavalink();

            //check if user is in valid channel
            if (ctx.Member.VoiceState == null || userVC == null)
            {
                await ctx.Channel.SendMessageAsync("Please join in valid voice channel first");
                return;
            }

            if (!lavalinkInstance.ConnectedNodes.Any())
            {
                await ctx.Channel.SendMessageAsync("Connection is not established");
                return;
            }

            if (userVC.Type != DSharpPlus.ChannelType.Voice)
            {
                await ctx.Channel.SendMessageAsync("Please enter a valid voice channel");
                return;
            }

            var node = lavalinkInstance.ConnectedNodes.Values.First();
            var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

            if (conn == null)
            {
                await ctx.Channel.SendMessageAsync("Lava link failed to connect");
                return;
            }

            if (conn.CurrentState.CurrentTrack == null)
            {
                await ctx.Channel.SendMessageAsync("No tracks are playing");
                return;
            }

            await conn.ResumeAsync();

            var resumedEmbed = new DiscordEmbedBuilder()
            {
                Color = DiscordColor.Green,
                Title = "Track resumed"
            };

            await ctx.Channel.SendMessageAsync(embed: resumedEmbed);
        }

        [Command("stop")]
        public async Task StopMusic(CommandContext ctx)
        {
            var userVC = ctx.Member?.VoiceState?.Channel;
            var lavalinkInstance = ctx.Client.GetLavalink();

            //check if user is in valid channel
            if (ctx.Member.VoiceState == null || userVC == null)
            {
                await ctx.Channel.SendMessageAsync("Please join in valid voice channel first");
                return;
            }

            if (!lavalinkInstance.ConnectedNodes.Any())
            {
                await ctx.Channel.SendMessageAsync("Connection is not established");
                return;
            }

            if (userVC.Type != DSharpPlus.ChannelType.Voice)
            {
                await ctx.Channel.SendMessageAsync("Please enter a valid voice channel");
                return;
            }

            var node = lavalinkInstance.ConnectedNodes.Values.First();
            var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

            if (conn == null)
            {
                await ctx.Channel.SendMessageAsync("Lava link failed to connect");
                return;
            }

            if (conn.CurrentState.CurrentTrack == null)
            {
                await ctx.Channel.SendMessageAsync("No tracks are playing");
                return;
            }

            await conn.StopAsync();
            await conn.DisconnectAsync();

            var stopEmbed = new DiscordEmbedBuilder()
            {
                Color = DiscordColor.Red,
                Title = "Disconnected from voice channel"
            };

            await ctx.Channel.SendMessageAsync(embed: stopEmbed);
        }

        [Command("help")]
        public async Task HelpMusic(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("Here are all my commands: \n" +
                                                "!play (music name from Youtube) \n" +
                                                "!pause \n" +
                                                "!resume \n" +
                                                "!stop \n" +
                                                "!volume (number)");
        }

        [Command("volume")]
        public async Task ChangeVolume(CommandContext ctx, int volume)
        {
            var userVC = ctx.Member?.VoiceState?.Channel;
            var lavalinkInstance = ctx.Client.GetLavalink();

            //check if user is in valid channel
            if (ctx.Member.VoiceState == null || userVC == null)
            {
                await ctx.Channel.SendMessageAsync("Please join in valid voice channel first");
                return;
            }

            if (!lavalinkInstance.ConnectedNodes.Any())
            {
                await ctx.Channel.SendMessageAsync("Connection is not established");
                return;
            }

            if (userVC.Type != DSharpPlus.ChannelType.Voice)
            {
                await ctx.Channel.SendMessageAsync("Please enter a valid voice channel");
                return;
            }

            var node = lavalinkInstance.ConnectedNodes.Values.First();
            var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

            if (conn == null)
            {
                await ctx.Channel.SendMessageAsync("Lava link failed to connect");
                return;
            }

            if (conn.CurrentState.CurrentTrack == null)
            {
                await ctx.Channel.SendMessageAsync("No tracks are playing");
                return;
            }

            await conn.SetVolumeAsync(volume);

            var volumeEmbed = new DiscordEmbedBuilder()
            {
                Color = DiscordColor.Purple,
                Title = $"Volume changed to {volume}"
            };

            await ctx.Channel.SendMessageAsync(embed: volumeEmbed);
        }
    }
}
