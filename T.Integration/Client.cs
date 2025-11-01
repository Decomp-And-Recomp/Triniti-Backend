using System.Net.Sockets;
using T.Integration.Protocol.SystemProtocol.ClientPackets;

namespace T.Integration;

internal class Client
{
    public ushort id;

    public readonly TcpClient tcpClient;
    public readonly NetworkStream stream;

    public IdentifyCmd? identifyData;

    public Client(TcpClient tcpClient)
    {
        this.tcpClient = tcpClient;
        stream = tcpClient.GetStream();

        _ = IdentifyCheck();
    }

    async Task IdentifyCheck()
    {
        await Task.Delay(15);

        if (identifyData == null) Disconnect(DisconnectCode.NotIdentified);
    }

    public void Disconnect(DisconnectCode code)
    {
        if (!Server.clients.TryRemove(id, out var v)) Logger.Error($"Unable to remove client '{id}' from the dictionary.");
        else if (v != this) Logger.Error($"When removing client '{id}' from the dictionary, some other client was removed.");

        tcpClient.Close();
    }
}