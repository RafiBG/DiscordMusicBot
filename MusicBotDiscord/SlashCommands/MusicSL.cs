using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using DSharpPlus.SlashCommands;

namespace MusicBotDiscord.SlashCommands
{
    public class MusicSL : ApplicationCommandModule
    {
        [SlashCommand("play", "Plays your YouTube music video")]
        public async Task PlayMusic(InteractionContext ctx, [Option("link_or_name","Enter the YouTube link or name")] string query)
        {
            await ctx.DeferAsync();

            // Why var userVC = ctx.Member?.VoiceState?.Channel; has this symbol ?
            // well if you are NOT in a voice channel it will crash
            // and trow null but this way with symbol ? will have null and continue
            // to the if where it will be done
            var userVC = ctx.Member?.VoiceState?.Channel;
            var lavalinkInstance = ctx.Client.GetLavalink();

            //check if user is in valid channel
            if (ctx.Member.VoiceState == null || userVC == null)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Please join in valid voice channel first"));
                return;
            }

            if (!lavalinkInstance.ConnectedNodes.Any())
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Connection is not established"));
                return;
            }

            if (userVC.Type != DSharpPlus.ChannelType.Voice)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Please enter a valid voice channel"));
                return;
            }

            // convert short YouTube URLs to full YouTube urls and remove additional query
            // parameter from (?) to the end from short version of the urls
            if (query.Contains("https://youtu.be/"))
            {
                var videoId = query.Replace("https://youtu.be/", "").Split('?')[0];
                query = $"https://www.youtube.com/watch?v={videoId}";
            }

            // convert YouTube Music URLs to full YouTube urls
            else if (query.Contains("https://music.youtube.com/"))
            {
                var videoId = query.Replace("https://music.youtube.com/", "").Split('&')[0];
                query = $"https://www.youtube.com/watch?v={videoId}";
            }

            // remove additional query parameter (&list) for standard full YouTube urls
            else if (query.Contains("https://www.youtube.com/watch?v="))
            {
                query = query.Split('&')[0];
            }

            var node = lavalinkInstance.ConnectedNodes.Values.First();
            await node.ConnectAsync(userVC);

            var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);
            if (conn == null || !conn.IsConnected)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Lavalink failed to connect"));
                return;
            }

            var searchQuery = await node.Rest.GetTracksAsync(query);
            if (searchQuery.LoadResultType == LavalinkLoadResultType.NoMatches || searchQuery.LoadResultType == LavalinkLoadResultType.LoadFailed)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Failed to find music with query: {query}"));
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

            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(nowPlayingEmbed));

            var noUsersInVCLeave = new DiscordEmbedBuilder()
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
                    if (conn != null && conn.IsConnected)
                    {
                        await conn.StopAsync();
                        await conn.DisconnectAsync();
                        await ctx.Channel.SendMessageAsync(embed: noUsersInVCLeave);
                        return;
                    }
                }
                await Task.Delay(10000);
            }
        }

        [SlashCommand("pause","For pausing the video")]
        public async Task PauseMusic(InteractionContext ctx)
        {
            await ctx.DeferAsync();

            var userVC = ctx.Member?.VoiceState?.Channel;
            var lavalinkInstance = ctx.Client.GetLavalink();

            //check if user is in valid channel
            if (ctx.Member.VoiceState == null || userVC == null)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Please join in valid voice channel first"));
                return;
            }

            if (!lavalinkInstance.ConnectedNodes.Any())
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Connection is not established"));
                return;
            }

            if (userVC.Type != DSharpPlus.ChannelType.Voice)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Please enter a valid voice channel"));
                return;
            }

            var node = lavalinkInstance.ConnectedNodes.Values.First();
            var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

            if (conn == null)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Lava link failed to connect"));
                return;
            }

            if (conn.CurrentState.CurrentTrack == null)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("No tracks are playing"));
                return;
            }

            await conn.PauseAsync();

            var pausedEmbed = new DiscordEmbedBuilder()
            {
                Color = DiscordColor.Yellow,
                Title = "Track paused"
            };

            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(pausedEmbed));
        }

        [SlashCommand("resume", "It resumes the video")]
        public async Task ResumeMusic(InteractionContext ctx)
        {
            await ctx.DeferAsync();

            var userVC = ctx.Member?.VoiceState?.Channel;
            var lavalinkInstance = ctx.Client.GetLavalink();

            //check if user is in valid channel
            if (ctx.Member.VoiceState == null || userVC == null)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Please join in valid voice channel first"));
                return;
            }

            if (!lavalinkInstance.ConnectedNodes.Any())
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Connection is not established"));
                return;
            }

            if (userVC.Type != DSharpPlus.ChannelType.Voice)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Please enter a valid voice channel"));
                return;
            }

            var node = lavalinkInstance.ConnectedNodes.Values.First();
            var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

            if (conn == null)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Lava link failed to connect"));
                return;
            }

            if (conn.CurrentState.CurrentTrack == null)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("No tracks are playing"));
                return;
            }

            await conn.ResumeAsync();

            var resumedEmbed = new DiscordEmbedBuilder()
            {
                Color = DiscordColor.Green,
                Title = "Track resumed"
            };

            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(resumedEmbed));
        }

        [SlashCommand("stop", "The bot leaves the voice chat")]
        public async Task StopMusic(InteractionContext ctx)
        {
            await ctx.DeferAsync();

            var userVC = ctx.Member?.VoiceState?.Channel;
            var lavalinkInstance = ctx.Client.GetLavalink();

            //check if user is in valid channel
            if (ctx.Member.VoiceState == null || userVC == null)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Please join in valid voice channel first"));
                return;
            }

            if (!lavalinkInstance.ConnectedNodes.Any())
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Connection is not established"));
                return;
            }

            if (userVC.Type != DSharpPlus.ChannelType.Voice)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Please enter a valid voice channel"));
                return;
            }

            var node = lavalinkInstance.ConnectedNodes.Values.First();
            var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

            if (conn == null)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Lava link failed to connect"));
                return;
            }

            if (conn.CurrentState.CurrentTrack == null)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("No tracks are playing"));
                return;
            }

            await conn.StopAsync();
            await conn.DisconnectAsync();

            var stopEmbed = new DiscordEmbedBuilder()
            {
                Color = DiscordColor.Red,
                Title = "Disconnected from voice channel"
            };

            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(stopEmbed));
        }

        [SlashCommand("volume", "Changes the video volume")]
        public async Task ChangeVolume(InteractionContext ctx, [Option("number", "Enter a volume number")] long volume)
        {
            await ctx.DeferAsync();

            var userVC = ctx.Member?.VoiceState?.Channel;
            var lavalinkInstance = ctx.Client.GetLavalink();

            //check if user is in valid channel
            if (ctx.Member.VoiceState == null || userVC == null)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Please join in valid voice channel first"));
                return;
            }

            if (!lavalinkInstance.ConnectedNodes.Any())
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Connection is not established"));
                return;
            }

            if (userVC.Type != DSharpPlus.ChannelType.Voice)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Please enter a valid voice channel"));
                return;
            }

            var node = lavalinkInstance.ConnectedNodes.Values.First();
            var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

            if (conn == null)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Lava link failed to connect"));
                return;
            }

            if (conn.CurrentState.CurrentTrack == null)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("No tracks are playing"));
                return;
            }

            await conn.SetVolumeAsync(Convert.ToInt32(volume));

            var volumeEmbed = new DiscordEmbedBuilder()
            {
                Color = DiscordColor.Purple,
                Title = $"Volume changed to {volume}%"
            };

            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(volumeEmbed));
        }
    }
}
