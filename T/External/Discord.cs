using Discord;
using Discord.WebSocket;
using System.Collections.Concurrent;
using T.Logging;
using LogLevel = T.Logging.LogLevel;

namespace T.External;

public class Discord
{
    private static readonly DiscordSocketClient Client = new(new DiscordSocketConfig
    {
        GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
    });

    private static ConcurrentQueue<(DateTime, byte[])>? LogQueue;
    private static System.Timers.Timer? LogTimer;

    public static async Task Run()
    {
        if (string.IsNullOrWhiteSpace(Config.Discord.Token))
        {
            Logger.Warning("Could not initialize Discord bot: Empty Token");
            return;
        }

        if (Config.Discord.ServerId == 0)
        {
            Logger.Warning("Could not initialize Discord bot: serverId cannot be 0");
            return;
        }

        if (Config.Discord.LoggingChannelId == 0)
        {
            Logger.Warning("Discord logging disabled: loggingChannelId cannot be 0");
        }
        else
        {
            LogTimer = new(800);
            LogQueue = new();
        }

        if (Config.Discord.AllowedRoles.Count == 0)
        {
            Logger.Warning("Could not initialize Discord bot: allowedRoles is empty.");
            return;
        }

        Client.Ready += OnReady;
        Client.Log += OnDiscordLog;
        Client.InteractionCreated += OnInteraction;

        await Client.LoginAsync(TokenType.Bot, Config.Discord.Token);
        await Client.StartAsync();
    }

    private static async Task OnReady()
    {
        try
        {
            if (LogTimer != null)
            {
                LogTimer.Elapsed += async (_, _) => await FlushLogQueue();
                LogTimer.AutoReset = true;
                LogTimer.Start();
            }

            var guild = Client.GetGuild(Config.Discord.ServerId);

            if (guild == null)
            {
                Logger.Warning($"A guild with id '{Config.Discord.ServerId}' cannot be found.");
                return;
            }

            await guild.CreateApplicationCommandAsync(new SlashCommandBuilder().WithName("config-reload")
                .WithDescription("Reloads configs from the Config folder.")
                .Build());

            await guild.CreateApplicationCommandAsync(new SlashCommandBuilder().WithName("say")
                .AddOption("message", ApplicationCommandOptionType.String, "The message.", isRequired: true)
                .WithDescription("Says the message.")
                .Build());

            await guild.CreateApplicationCommandAsync(new SlashCommandBuilder().WithName("ban-ip")
                .AddOption("ip", ApplicationCommandOptionType.String, "Hashed IP address to ban.", isRequired: true)
                .WithDescription("Bans an IP.")
                .Build());

            await guild.CreateApplicationCommandAsync(new SlashCommandBuilder().WithName("ban-hwid")
                .AddOption("hwid", ApplicationCommandOptionType.String, "Hardware ID to ban.", isRequired: true)
                .WithDescription("Bans an HWID.")
                .Build());
        }
        catch (Exception ex)
        {
            Logger.Exception(ex);
        }
    }

    private static async Task OnInteraction(SocketInteraction interaction)
    {
        if (interaction is not SocketSlashCommand cmd) return;

        if (interaction.GuildId != Config.Discord.ServerId)
        {
            await cmd.RespondAsync("Server id does not match one in the config. Please set a proper ID in the config.");
            return;
        }

        bool allowed = false;

        if (cmd.User is not SocketGuildUser user) return;

        foreach (var v in user.Roles)
        {
            if (!Config.Discord.AllowedRoles.Contains(v.Id)) continue;

            allowed = true;
            break;
        }

        if (!allowed)
        {
            await cmd.RespondAsync("No permission.", ephemeral: true);
            return;
        }

        try
        {
            switch (cmd.Data.Name)
            {
                case "config-reload":
                    await cmd.RespondAsync("Reloading config...");
                    await Config.Load();
                    await cmd.ModifyOriginalResponseAsync(msg =>
                    {
                        msg.Content = $"Config reloaded.";
                    });
                    return;
                case "say":
                    string msg = (string)cmd.Data.Options.First(f => f.Name == "message").Value;
                    await cmd.Channel.SendMessageAsync(msg);
                    await cmd.RespondAsync("Done.", ephemeral: true);
                    Logger.Info($"{cmd.User.GlobalName} said: '{msg}' in '{cmd.Channel.Name}'");
                    return;
                case "ban-ip":
                    string ip = (string)cmd.Data.Options.First(f => f.Name == "ip").Value;
                    await DB.BanDatabase.BanIp(ip);
                    await cmd.RespondAsync("Done.");
                    return;
                case "ban-hwid":
                    string hwid = (string)cmd.Data.Options.First(f => f.Name == "ip").Value;
                    await DB.BanDatabase.BanHWID(hwid);
                    await cmd.RespondAsync("Done.");
                    return;
            }
        }
        catch (Exception ex)
        {
            await cmd.RespondAsync("An exception occured on the server.", ephemeral: true);
            Logger.Exception(ex);
        }
    }

    private static Task OnDiscordLog(LogMessage msg)
    {
        LogLevel logLevel;

        switch (msg.Severity)
        {
            case LogSeverity.Critical: logLevel = LogLevel.Critical; break;
            case LogSeverity.Error: logLevel = LogLevel.Error; break;
            //case LogSeverity.Warning: logLevel = LogLevel.Warning; break;
            //case LogSeverity.Info: logLevel = LogLevel.Info; break;
            //case LogSeverity.Verbose: break;
            case LogSeverity.Debug: logLevel = LogLevel.Log; break;
            default: return Task.CompletedTask;
        }

        Logger.Log(logLevel, msg.ToString());
        return Task.CompletedTask;
    }

    public static void Log(string? message)
    {
        if (LogQueue == null) return;
        if (string.IsNullOrWhiteSpace(message)) return;
        LogQueue.Enqueue((DateTime.Now, System.Text.Encoding.UTF8.GetBytes(message)));
    }

    private static async Task FlushLogQueue()
    {
        if (LogQueue!.IsEmpty) return;

        try
        {
            if (Client.LoginState != LoginState.LoggedIn) return;

            var channel = await Client.GetChannelAsync(Config.Discord.LoggingChannelId);
            if (channel is not IMessageChannel msgChannel) return;

            if (LogQueue.TryDequeue(out var v))
            {
                using MemoryStream stream = new(v.Item2);

                await msgChannel.SendFileAsync(stream, filename: $"{v.Item1.Year}_{v.Item1.Month}_{v.Item1.Day}_{v.Item1.Hour}:{v.Item1.Minute}:{v.Item1.Second}.txt");
            }
        }
        catch (Exception ex)
        {
            Logger.Exception(ex);
        }
    }
}