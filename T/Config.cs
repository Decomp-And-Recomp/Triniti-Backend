using T.External.SimpleJSON;

namespace T;

internal static class Config // THIS IS TEMP IM GONNA REWRITE IT
{
	static string configPath = "config.json";

	public static string mySqlConnectionString { get; private set; } = string.Empty;
	public static string hostUrl { get; private set; } = string.Empty;

	//DH
	public static int dhLeaderboardReturnAmount { get; private set; }

	public static void Init()
	{
        configPath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)!, configPath);

        if (!File.Exists(configPath))
		{
			Debug.Log("Unable to find config, loading defaults.");

			InitDefault();
			return;
		}

        string configFile = File.ReadAllText(configPath);

		JSONNode index = JSON.Parse(configFile);

		mySqlConnectionString = index["mySqlConnectionString"];
		hostUrl = index["hostUrl"];

		// DH
		dhLeaderboardReturnAmount = index["dhl"];
	}

	static void InitDefault()
	{
		mySqlConnectionString = "Server=127.0.0.1;Port=3306;User ID=root;Password=;Database=Triniti;";
		hostUrl = "http://127.0.0.1:7125";
		dhLeaderboardReturnAmount = 20;

		JSONObject index = new();

		index["mySqlConnectionString"] = mySqlConnectionString;
		index["hostUrl"] = hostUrl;

		index["dhl"] = dhLeaderboardReturnAmount;

		if (File.Exists(configPath)) File.Delete(configPath);

		using var sw = File.CreateText(configPath);

		sw.Write(index.ToString(1));
		sw.Close();
	}
}
