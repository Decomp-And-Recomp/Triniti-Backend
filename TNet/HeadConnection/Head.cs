using System.Net.Sockets;
using System.Net;

namespace TNet.HeadConnection;

internal static class Head
{
    static int instanceId;

    public static TcpClient? client;

    public static async Task Start(int connectPort, int instanceId)
    {
        Head.instanceId = instanceId;

        if (client != null)
        {
            Debug.LogWarning("Head connection already started.");
            return;
        }

        client = new TcpClient();

        await client.ConnectAsync(IPAddress.Any, connectPort);


    }
}
