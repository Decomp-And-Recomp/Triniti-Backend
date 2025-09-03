using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
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

        return XXTEAUtils.Decrypt(data, Config.encryptionKey);
    }

    public static string Encrypt(string str)
    {
        return XXTEAUtils.Encrypt(str, Config.encryptionKey);
    }
}
