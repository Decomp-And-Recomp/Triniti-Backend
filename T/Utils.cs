using System.Security.Cryptography;
using System.Text;
using T.External;

namespace T;

public static class Utils
{
    public static string? GetIp(HttpContext context)
    {
        string? ip = context.Request.Headers["HTTP_X_FORWARDED_FOR"].FirstOrDefault();

        if (string.IsNullOrEmpty(ip))
        {
            var ipAddr = context.Connection.RemoteIpAddress;

            if (ipAddr == null) return null;

            ip = ipAddr.IsIPv4MappedToIPv6 ? ipAddr.MapToIPv4().ToString() : ipAddr.ToString();
        }

        return Hash(ip);
    }

    public static string Hash(string input)
        => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(input)));

    public static async Task<string> ReadEncryptedBody(HttpRequest request)
    {
        StreamReader reader = new(request.Body);
        string data = await reader.ReadToEndAsync();

        if (string.IsNullOrWhiteSpace(Config.General.EncryptionKey)) return data;

        return XXTEAUtils.Decrypt(data, Config.General.EncryptionKey);
    }

    public static string Encrypt(string str)
    {
        if (string.IsNullOrWhiteSpace(Config.General.EncryptionKey)) return str;

        return XXTEAUtils.Encrypt(str, Config.General.EncryptionKey);
    }
}
