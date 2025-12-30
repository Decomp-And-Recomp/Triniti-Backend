using System.Net;

namespace T.Integration;

public static class Config
{
    public static IPAddress? hostAddress;
    public static int port;
    public static string encryptionKey = string.Empty;

    public static int timeout = 60;
}
