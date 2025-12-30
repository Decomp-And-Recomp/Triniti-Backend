using System.Text.Json.Nodes;
using T.Logging;

namespace T;

public static class Config
{
    private static string ConfigPath = "Configs/config.json";

    public static class General
    {
        public static string HostUrl = string.Empty;
        public static string EncryptionKey = string.Empty;
        public static bool EnableAntiCheat;
    }

    public static class Database
    {
        public enum DatabaseType { MySQL }

        public static DatabaseType Type;

        public static string Server = string.Empty;
        public static int Port;
        public static string UserId = string.Empty;
        public static string Password = string.Empty;
        public static string DatabaseName = string.Empty;
    }

    public static class Discord
    {
        public static string Token = string.Empty;
        public static ulong ServerId;
        public static ulong LoggingChannelId;
        public static List<ulong> AllowedRoles = [];
    }

    public static class DinoHunter
    {
        public static int MaxLeaderboardReturnAmount;
    }

    public static async Task Initialize()
    {
        var assemblyPath = System.Reflection.Assembly.GetExecutingAssembly().Location;

        if (string.IsNullOrEmpty(assemblyPath)) throw new Exception("Error loading configs, assembly path is null.");

        ConfigPath = Path.Combine(Path.GetDirectoryName(assemblyPath)!, ConfigPath);

        string configDirectory = Path.GetDirectoryName(ConfigPath)!;

        if (!Directory.Exists(configDirectory)) Directory.CreateDirectory(configDirectory);

        if (!File.Exists(ConfigPath))
        {
            Logger.Warning("Config file not found, creating default one.");
            SetDefaults();
            await Save();
            return;
        }

        await Load();
    }

    public static void SetDefaults()
    {
        General.HostUrl = "http://127.0.0.1:7125";
        General.EncryptionKey = "ExampleKey";
        General.EnableAntiCheat = false;

        Database.Type = Database.DatabaseType.MySQL;

        Database.Server = "127.0.0.1";
        Database.Port = 3306;
        Database.UserId = "root";
        Database.Password = string.Empty;
        Database.DatabaseName = "Triniti";

        Discord.Token = string.Empty;
        Discord.ServerId = 0;
        Discord.LoggingChannelId = 0;
        Discord.AllowedRoles.Clear();

        DinoHunter.MaxLeaderboardReturnAmount = 200;
    }

    public static async Task Save()
    {
        JsonNode index = new JsonObject();

        JsonNode general = new JsonObject();
        JsonNode database = new JsonObject();
        JsonNode discord = new JsonObject();
        JsonNode dinoHunter = new JsonObject();

        index["general"] = general;
        index["database"] = database;
        index["discord"] = discord;
        index["dinoHunter"] = dinoHunter;

        general["hostUrl"] = General.HostUrl;
        general["encryptionKey"] = General.EncryptionKey;
        general["enableAntiCheat"] = General.EnableAntiCheat;

        database["type"] = Database.Type switch
        {
            Database.DatabaseType.MySQL => (JsonNode)"MySQL",
            _ => (JsonNode)"undefined",
        };

        database["server"] = Database.Server;
        database["port"] = Database.Port;
        database["userId"] = Database.UserId;
        database["password"] = Database.Password;
        database["databaseName"] = Database.DatabaseName;

        discord["token"] = Discord.Token;
        discord["serverId"] = Discord.ServerId;
        discord["loggingChannelId"] = Discord.LoggingChannelId;
        
        JsonArray allowedRoles = [.. Discord.AllowedRoles];
        discord["allowedRoles"] = allowedRoles;

        dinoHunter["maxLeaderboardReturnAmount"] = DinoHunter.MaxLeaderboardReturnAmount;

        await File.WriteAllTextAsync(ConfigPath, index.ToJsonString(new System.Text.Json.JsonSerializerOptions() { WriteIndented = true }));
    }

    public static async Task Load()
    {
        string configFile = await File.ReadAllTextAsync(ConfigPath);

        JsonNode index = JsonNode.Parse(configFile)
            ?? throw new Exception("Unable to parse config file: Make sure its a valid JSON.");

        JsonNode? general = GetNode(index, "general");
        JsonNode? database = GetNode(index, "database");
        JsonNode? discord = GetNode(index, "discord");
        JsonNode? dinoHunter = GetNode(index, "dinoHunter");

        // General
        General.HostUrl = GetNode(general, "hostUrl").GetValue<string>();
        General.EncryptionKey = GetNode(general, "encryptionKey").GetValue<string>();
        General.EnableAntiCheat = GetNode(general, "enableAntiCheat").GetValue<bool>();

        // Database
        string databaseType = GetNode(database, "type").GetValue<string>();

        Database.Type = databaseType switch
        {
            "MySQL" => Database.DatabaseType.MySQL,
            _ => throw new Exception($"{databaseType} is unacceptable value. Only 'MySQL' is currently supported"),
        };
        Database.Server = GetNode(database, "server").GetValue<string>();
        Database.Port = GetNode(database, "port").GetValue<int>();
        Database.UserId = GetNode(database, "userId").GetValue<string>();
        Database.Password = GetNode(database, "password").GetValue<string>();
        Database.DatabaseName = GetNode(database, "databaseName").GetValue<string>();

        // Discord
        JsonArray allowedRoleIds = (JsonArray)GetNode(discord, "allowedRoles");

        foreach (var v in allowedRoleIds)
        {
            if (v == null) throw new Exception("An entry in allowedUsers is null.");

            Discord.AllowedRoles.Add(v.GetValue<ulong>());
        }

        Discord.ServerId = GetNode(discord, "serverId").GetValue<ulong>();
        Discord.Token = GetNode(discord, "token").GetValue<string>();
        Discord.LoggingChannelId = GetNode(discord, "loggingChannelId").GetValue<ulong>();

                                // Games
        // DinoHunter
        DinoHunter.MaxLeaderboardReturnAmount = GetNode(dinoHunter, "maxLeaderboardReturnAmount").GetValue<int>();
        if (DinoHunter.MaxLeaderboardReturnAmount < 0) throw new Exception("maxLeaderboardReturnAmount CAN NOT be negative.");
    }

    static JsonNode GetNode(JsonNode from, string node)
    {
        return from[node] ?? throw new Exception($"Unable to parse config file: the '{node}' node was not found at {from.GetPath()}");
    }
}
