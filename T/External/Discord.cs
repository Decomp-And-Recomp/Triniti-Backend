using Discord;
using Discord.WebSocket;
using System.Collections.Concurrent;
using T.Logging;
using LogLevel = T.Logging.LogLevel;

namespace T.External;

public class Discord
{
    static readonly DiscordSocketClient client = new(new DiscordSocketConfig
    {
        GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
    });

    static ConcurrentQueue<string>? logQueue;
    static System.Timers.Timer? logTimer;

    public static async Task Run()
    {
        if (string.IsNullOrWhiteSpace(Config.Discord.token))
        {
            Logger.Warning("Could not initialize Discord bot: Empty Token");
            return;
        }

        if (Config.Discord.serverId == 0)
        {
            Logger.Warning("Could not initialize Discord bot: serverId cannot be 0");
            return;
        }

        if (Config.Discord.loggingChannelId == 0)
        {
            Logger.Warning("Discord logging disabled: loggingChannelId cannot be 0");
        }
        else
        {
            logTimer = new(800);
            logQueue = new();
        }

        if (Config.Discord.allowedRoles.Count == 0)
        {
            Logger.Warning("Could not initialize Discord bot: allowedRoles is empty.");
            return;
        }

        client.Ready += OnReady;
        client.Log += OnDiscordLog;
        client.InteractionCreated += OnInteraction;

        await client.LoginAsync(TokenType.Bot, Config.Discord.token);
        await client.StartAsync();
    }

    static async Task OnReady()
    {
        try
        {
            if (logTimer != null)
            {
                logTimer.Elapsed += async (_, _) => await FlushLogQueue();
                logTimer.AutoReset = true;
                logTimer.Start();
            }

            var guild = client.GetGuild(Config.Discord.serverId);

            if (guild == null)
            {
                Logger.Warning($"A guild with id '{Config.Discord.serverId}' cannot be found.");
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
            Logger.LogException(ex);
        }
    }

    static async Task OnInteraction(SocketInteraction interaction)
    {
        if (interaction is not SocketSlashCommand cmd) return;

        if (interaction.GuildId != Config.Discord.serverId)
        {
            await cmd.RespondAsync("Server id does not match one in the config. Please set a proper ID in the config.");
            return;
        }

        bool allowed = false;

        if (cmd.User is not SocketGuildUser user) return;

        foreach (var v in user.Roles)
        {
            if (!Config.Discord.allowedRoles.Contains(v.Id)) continue;

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
                    await DB.banDatabase.BanIp(ip);
                    await cmd.RespondAsync("Done.");
                    return;
                case "ban-hwid":
                    string hwid = (string)cmd.Data.Options.First(f => f.Name == "ip").Value;
                    await DB.banDatabase.BanHWID(hwid);
                    await cmd.RespondAsync("Done.");
                    return;
            }
        }
        catch (Exception ex)
        {
            await cmd.RespondAsync("An exception occured on the server.", ephemeral: true);
            Logger.LogException(ex);
        }
    }

    static Task OnDiscordLog(LogMessage msg)
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
        if (logQueue == null) return;
        if (string.IsNullOrWhiteSpace(message)) return;
        logQueue.Enqueue(message);
    }

    static async Task FlushLogQueue()
    {
        if (logQueue!.IsEmpty) return;

        try
        {
            if (client.LoginState != LoginState.LoggedIn) return;

            var channel = await client.GetChannelAsync(Config.Discord.loggingChannelId);
            if (channel is not IMessageChannel msgChannel) return;

            if (logQueue.TryDequeue(out var message))
            {
                if (message.Length > 1900)
                    message = message[..1900] + "...";

                await msgChannel.SendMessageAsync($"```{message}```");
            }
        }
        catch (Exception ex)
        {
            Logger.LogException(ex);
        }
    }
}