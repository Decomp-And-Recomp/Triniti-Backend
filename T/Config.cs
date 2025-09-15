using T.External.SimpleJSON;

namespace T;

internal static class Config // THIS IS TEMP IM GONNA REWRITE IT
{
    static string configPath = "config.json";

    public static string mySqlConnectionString { get; private set; } = "Server=127.0.0.1;Port=3306;User ID=root;Password=;Database=Triniti;";

    public static string hostUrl { get; private set; } = "http://127.0.0.1:7125";
    public static string encryptionKey { get; private set; } = string.Empty;
    public static string webhook { get; private set; } = string.Empty;
    public static bool enableAntiCheat { get; private set; }

    //DH
    public static int dhLeaderboardReturnAmount { get; private set; } = 20;

    public static void Init()
    {
        configPath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)!, configPath);

        if (!File.Exists(configPath))
        {
            Debug.Log("Unable to find config, loading defaults.");

            Save();
            return;
        }

        string configFile = File.ReadAllText(configPath);

        JSONNode index = JSON.Parse(configFile);

        mySqlConnectionString = index["mySqlConnectionString"];
        webhook = index["webhook"];
        hostUrl = index["hostUrl"];
        enableAntiCheat = index["enableAntiCheat"];

        JSONObject defaultEncryptionkey = new();
        defaultEncryptionkey.Value = "ExampleKey";

        encryptionKey = index.GetValueOrDefault("encryptionKey", defaultEncryptionkey);

        // DH
        dhLeaderboardReturnAmount = index["dhl"];

        Save();
    }

    static void Save()
    {
        JSONObject index = new();

        index["mySqlConnectionString"] = mySqlConnectionString;
        index["hostUrl"] = hostUrl;

        index["encryptionKey"] = encryptionKey;
        index["webhook"] = webhook;
        index["enableAntiCheat"] = enableAntiCheat;

        index["dhl"] = dhLeaderboardReturnAmount;
        if (File.Exists(configPath)) File.Delete(configPath);

        using var sw = File.CreateText(configPath);

        sw.Write(index.ToString(1));
        sw.Close();
    }
}
