using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using DSharpPlus.Lavalink;
using DSharpPlus.Net;
using DSharpPlus.SlashCommands;
using MusicBotDiscord.Config;
using MusicBotDiscord.SlashCommands;

namespace MusicBotDiscord;
public class Program
{
    public static DiscordClient Client { get; set; }
    public static InteractivityExtension Intereaction { get; private set; }
    public static CommandsNextExtension Commands { get; set; }
    static async Task Main(string[] args)
    {
        var jsonReader = new JSONReader();
        await jsonReader.ReadJSON();

        if (string.IsNullOrWhiteSpace(jsonReader.token));
        {
            Console.WriteLine("The token in config.json is missing or invalid. Please provide a valid bot token.");
        }
        var discordConfig = new DiscordConfiguration()
        {
            Intents = DiscordIntents.All,
            Token = jsonReader?.token,
            TokenType = TokenType.Bot,
            AutoReconnect = true,
        };

        Client = new DiscordClient(discordConfig);

        Client.Ready += Client_Ready;

        var commandsConfig = new CommandsNextConfiguration()
        {
            StringPrefixes = new string[] { jsonReader.prefix },
            EnableMentionPrefix = true,
            EnableDms = true,
            EnableDefaultHelp = false
        };
        
        Commands = Client.UseCommandsNext(commandsConfig);
        // The line under if you want to use prefix uncomment it
        //Commands.RegisterCommands<MusicCommands>(); //register commands with prefix symbol  ! 

        var slashCommandConfig = Client.UseSlashCommands();
        // For specific server also discord updates new changes faster
        //slashCommandConfig.RegisterCommands<MusicSL>(guildId: 1317235767319199845);

        // This may take up to 1 hour for discord to see the new changes and make it work
        slashCommandConfig.RegisterCommands<MusicSL>();

        // Lavalink configuration

        //If there is this error
        //[Crit ] Failed to connect to Lavalink
        //Change to other server or create one link : https://lavalink.darrennathanael.com/SSL/lavalink-with-ssl/#tunnelbroker-guide
        var endpoint = new ConnectionEndpoint
        {
            Hostname = jsonReader.hostName,
            Port = jsonReader.port,
            Secured = jsonReader.secured,
        };

        var lavalinkConfig = new LavalinkConfiguration
        {
            Password = jsonReader.password,
            RestEndpoint = endpoint,
            SocketEndpoint = endpoint,
        };

        var lavalink = Client.UseLavalink();

        // connect for the Bot to be online
        await Client.ConnectAsync();
        await lavalink.ConnectAsync(lavalinkConfig);
        await Task.Delay(-1);
    }
    private static Task Client_Ready(DiscordClient sender, DSharpPlus.EventArgs.ReadyEventArgs e)
    {
        //Console.WriteLine("Bot is ready");
        return Task.CompletedTask;
    }
}