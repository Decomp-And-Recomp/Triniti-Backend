using System.Text;
using System.Text.Json.Nodes;
using System.Xml;
using T.Database.Objects.DinoHunter;
using T.Logging;

namespace T.Extensions;

public static class DinoHunterExtensions
{
    public static JsonNode ToJson(this AccountEntry entry)
    {
        return new JsonObject()
        {
            ["code"] = "0", // A requirement by the DinoHunter code.
            ["userId"] = entry.userId,
            ["nickName"] = entry.nickname,
            ["title"] = entry.title.ToString(),
            ["exts"] = entry.exts
        };
    }

    public static string? GetMottoFromExts(this AccountEntry entry)
    {
        if (entry.exts == null) return null;

        try
        {
            // substring is a bandaid fix
            string xml = Encoding.UTF8.GetString(Convert.FromBase64String(entry.exts))[1..];
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
            string extsConvert = Encoding.UTF8.GetString(Convert.FromBase64String(entry.exts));

            Logger.Log(extsConvert);
            Logger.Log(entry.exts);
            Logger.Exception(ex);
        }

        return "Internal Error";
    }

    public static void SetMottoToExts(this AccountEntry entry, string motto)
    {
        if (entry.exts == null) return;
        try
        {
            // substring is a bandaid fix
            string xml = Encoding.UTF8.GetString(Convert.FromBase64String(entry.exts))[1..];
            XmlDocument doc = new();
            doc.LoadXml(xml);

            var cncPack = doc["CNCPack"];
            if (cncPack == null) return;

            var sign = cncPack["sign"];
            if (sign == null) return;

            sign.InnerText = motto;

            // exts uploaded by game has the "77u\n (yes its new line)" at begenning? but when adding it manualy causes exception on game?
            // meh, works fine rn so i wont even bother debugging it.
            //exts = "77u/\n" + Convert.ToBase64String(Encoding.UTF8.GetBytes(doc.OuterXml));
            entry.exts = Convert.ToBase64String(Encoding.UTF8.GetBytes(doc.OuterXml));
        }
        catch (Exception ex)
        {
            string extsConvert = Encoding.UTF8.GetString(Convert.FromBase64String(entry.exts));

            Logger.Log(extsConvert);
            Logger.Log(entry.exts);
            Logger.Exception(ex);
        }
    }
}
