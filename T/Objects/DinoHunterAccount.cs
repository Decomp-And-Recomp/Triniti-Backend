using System.Text;
using System.Text.Json.Nodes;
using System.Xml;
using T.External.SimpleJSON;

namespace T.Objects;

public class DinoHunterAccount
{
	// SV = save request, LB = leaderboard request
	public string? userId; // SV | LB
	public string? nickname; // SV | LB
	public int? title; // SV
	public string? exts; // SV

	public int? combatpower; // LB
	public int? exp; // LB
	public int? hunterLv; // LB
	public int? crystal; // LB
	public int? gold; // LB
	public int? applause; // LB

	public static DinoHunterAccount FromJson(string json, bool leaderboard)
	{
		DinoHunterAccount result = new();

		JSONNode index = JSON.Parse(json);

		result.userId = index["userId"];
		result.nickname = index["nickName"]; // yes its right, nickName
		
		if (leaderboard)
		{
			result.combatpower = index["combatpower"];
			result.exp = index["exp"];
			result.hunterLv = index["hunterLv"];
			result.crystal = index["crystal"];
			result.gold = index["gold"];
			result.applause = index["applause"];

			return result;
		}
		
		result.title = index["title"].AsInt;
		result.exts = index["exts"];

		return result;
	}

	public JsonObject ToJson(bool includeCode)
	{
		JsonObject index = new();

		if (includeCode) index["code"] = "0";

		if (userId != null) index["userId"] = userId;
		if (nickname != null) index["nickName"] = nickname; // yes, nickName
		if (title != null) index["title"] = title.ToString();
		if (exts != null) index["exts"] = exts;

		return index;
	}

	public string? GetMottoFromExts()
	{
		if (exts == null) return null;

        try
        {
			// substring is a bandaid fix
            string xml = Encoding.UTF8.GetString(Convert.FromBase64String(exts)).Substring(1);
            XmlDocument doc = new();
            doc.LoadXml(xml);

            var cncPack = doc["CNCPack"];
            if (cncPack == null) return null;

            var sign = cncPack["sign"];
            if (sign == null) return null;

            return sign.InnerText;
        }
        catch (Exception ex)
        {
			Debug.Log(Encoding.UTF8.GetString(Convert.FromBase64String(exts)), ConsoleColor.DarkYellow);
			Debug.Log(exts, ConsoleColor.DarkRed);
			Debug.LogException(ex);
        }

        return "Internal Error";
    }

	public void SetMottoToExts(string motto)
	{
		if (exts == null) return;
		try
        {
            // substring is a bandaid fix
            string xml = Encoding.UTF8.GetString(Convert.FromBase64String(exts)).Substring(1);
            XmlDocument doc = new();
            doc.LoadXml(xml);

            var cncPack = doc["CNCPack"];
            if (cncPack == null) return;

            var sign = cncPack["sign"];
            if (sign == null) return;

            sign.InnerText = motto;

            exts = "77u/\n" + Convert.ToBase64String(Encoding.UTF8.GetBytes(doc.OuterXml));
        }
		catch (Exception ex)
        {
            Debug.Log(Encoding.UTF8.GetString(Convert.FromBase64String(exts)), ConsoleColor.DarkYellow);
            Debug.Log(exts, ConsoleColor.DarkRed);
            Debug.LogException(ex);
        }
    }
}
