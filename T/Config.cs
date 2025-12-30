using System.Text.Json.Nodes;
using T.Logging;

namespace T;

public static class Config
{
    static string configPath = "Configs/config.json";

    public static class General
    {
        public static string hostUrl = string.Empty;
        public static string encryptionKey = string.Empty;
        public static bool enableAntiCheat;
    }

    public static class Database
    {
        public enum Type { MySQL }

        public static Type type;

        public static string server = string.Empty;
        public static int port;
        public static string userId = string.Empty;
        public static string password = string.Empty;
        public static string databaseName = string.Empty;
    }

    public static class Discord
    {
        public static string token = string.Empty;
        public static ulong serverId;
        public static ulong loggingChannelId;
        public static List<ulong> allowedRoles = [];
    }

    public static class DinoHunter
    {
        public static int maxLeaderboardReturnAmount;
    }

    public static async Task Initialize()
    {
        var assemblyPath = System.Reflection.Assembly.GetExecutingAssembly().Location;

        if (string.IsNullOrEmpty(assemblyPath)) throw new Exception("Error loading configs, assembly path is null.");

        configPath = Path.Combine(Path.GetDirectoryName(assemblyPath)!, configPath);

        string configDirectory = Path.GetDirectoryName(configPath)!;

        if (!Directory.Exists(configDirectory)) Directory.CreateDirectory(configDirectory);

        if (!File.Exists(configPath))
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
        General.hostUrl = "http://127.0.0.1:7125";
        General.encryptionKey = "ExampleKey";
        General.enableAntiCheat = false;

        Database.type = Database.Type.MySQL;

        Database.server = "127.0.0.1";
        Database.port = 3306;
        Database.userId = "root";
        Database.password = string.Empty;
        Database.databaseName = "Triniti";

        Discord.token = string.Empty;
        Discord.serverId = 0;
        Discord.loggingChannelId = 0;
        Discord.allowedRoles.Clear();

        DinoHunter.maxLeaderboardReturnAmount = 200;
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

        general["hostUrl"] = General.hostUrl;
        general["encryptionKey"] = General.encryptionKey;
        general["enableAntiCheat"] = General.enableAntiCheat;

        database["type"] = Database.type switch
        {
            Database.Type.MySQL => (JsonNode)"MySQL",
            _ => (JsonNode)"undefined",
        };

        database["server"] = Database.server;
        database["port"] = Database.port;
        database["userId"] = Database.userId;
        database["password"] = Database.password;
        database["databaseName"] = Database.databaseName;

        discord["token"] = Discord.token;
        discord["serverId"] = Discord.serverId;
        discord["loggingChannelId"] = Discord.loggingChannelId;
        
        JsonArray allowedRoles = [.. Discord.allowedRoles];
        discord["allowedRoles"] = allowedRoles;

        dinoHunter["maxLeaderboardReturnAmount"] = DinoHunter.maxLeaderboardReturnAmount;

        await File.WriteAllTextAsync(configPath, index.ToJsonString(new System.Text.Json.JsonSerializerOptions() { WriteIndented = true }));
    }

    public static async Task Load()
    {
        string configFile = await File.ReadAllTextAsync(configPath);

        JsonNode index = JsonNode.Parse(configFile)
            ?? throw new Exception("Unable to parse config file: Make sure its a valid JSON.");

        JsonNode? general = GetNode(index, "general");
        JsonNode? database = GetNode(index, "database");
        JsonNode? discord = GetNode(index, "discord");
        JsonNode? dinoHunter = GetNode(index, "dinoHunter");

        // General
        General.hostUrl = GetNode(general, "hostUrl").GetValue<string>();
        General.encryptionKey = GetNode(general, "encryptionKey").GetValue<string>();
        General.enableAntiCheat = GetNode(general, "enableAntiCheat").GetValue<bool>();

        // Database
        string databaseType = GetNode(database, "type").GetValue<string>();

        Database.type = databaseType switch
        {
            "MySQL" => Database.Type.MySQL,
            _ => throw new Exception($"{databaseType} is unacceptable value. Only 'MySQL' is currently supported"),
        };
        Database.server = GetNode(database, "server").GetValue<string>();
        Database.port = GetNode(database, "port").GetValue<int>();
        Database.userId = GetNode(database, "userId").GetValue<string>();
        Database.password = GetNode(database, "password").GetValue<string>();
        Database.databaseName = GetNode(database, "databaseName").GetValue<string>();

        // Discord
        JsonArray allowedRoleIds = (JsonArray)GetNode(discord, "allowedRoles");

        foreach (var v in allowedRoleIds)
        {
            if (v == null) throw new Exception("An entry in allowedUsers is null.");

            Discord.allowedRoles.Add(v.GetValue<ulong>());
        }

        Discord.serverId = GetNode(discord, "serverId").GetValue<ulong>();
        Discord.token = GetNode(discord, "token").GetValue<string>();
        Discord.loggingChannelId = GetNode(discord, "loggingChannelId").GetValue<ulong>();

                                // Games
        // DinoHunter
        DinoHunter.maxLeaderboardReturnAmount = GetNode(dinoHunter, "maxLeaderboardReturnAmount").GetValue<int>();
        if (DinoHunter.maxLeaderboardReturnAmount < 0) throw new Exception("maxLeaderboardReturnAmount CAN NOT be negative.");
    }

    static JsonNode GetNode(JsonNode from, string node)
    {
        return from[node] ?? throw new Exception($"Unable to parse config file: the '{node}' node was not found at {from.GetPath()}");
    }
}
