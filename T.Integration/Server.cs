using System.Collections.Concurrent;
using System.Net.Sockets;
using T.Integration.Helpers;
using T.Integration.Protocol;
using T.Integration.Protocol.SystemProtocol;

namespace T.Integration;

internal static class Server
{
    const int maxPacketLength = 4096;
    const int headerLength = 8;

    static TcpListener? listener;

    public static ConcurrentDictionary<ulong, object> clients = [];
    static ulong curId;

    static readonly ProtocolHandler[] handlers =
    {
        new SystemProtocolHandler()
    };

    public static async Task Run()
    {
        if (Config.hostAddress == null)
            throw new ArgumentException($"The field '{nameof(Config.hostAddress)}' cannot be null.");

        listener = new(Config.hostAddress, Config.port);

        Logger.Info($"Integration server starting on ip '{Config.hostAddress}' and port '{Config.port}'.");

        listener.Start();

        Logger.Info("Server loop started.");

        while (true)
        {
            _ = HandleClient(await listener.AcceptTcpClientAsync());
        }
    }

    static async Task HandleClient(TcpClient tcpClient)
    {
        Client client = new(tcpClient);

        while (true)
        {
            if (clients.TryAdd(curId, client)) break;
            curId++;
        }

        byte[] buffer = new byte[maxPacketLength];
        List<byte> received = [];

        int read;
        int length = 0;
        bool lengthCalculated = false;

        while (true)
        {
            try
            {
                read = await client.stream.ReadAsync(buffer);
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
                client.Disconnect(DisconnectCode.ReadException);


                return;
            }

            if (read == 0)
            {
                client.Disconnect(DisconnectCode.SocketDisconnect);
                return;
            }

            received.AddRange(buffer.Take(read));


            while (received.Count > headerLength)
            {
                if (lengthCalculated)
                {
                    if (received.Count < length) break;
                }
                else
                {
                    EncryptionHelper.DecryptHeader(received);

                    length = ((received[0] << 8) | received[1]);

                    if (length < headerLength || length > maxPacketLength)
                    {
                        Logger.Error($"Length received: {length}");
                        client.Disconnect(DisconnectCode.DataOutOfBounds);
                        return;
                    }

                    lengthCalculated = true;
                    continue;
                }

                var takenData = received.Take(length).ToArray();
                received.RemoveRange(0, length);

                lengthCalculated = false;

                try
                {
                    HandlePacket(client, takenData);
                }
                catch (Exception ex)
                {
                    client.Disconnect(DisconnectCode.PacketException);
                    Logger.Exception(ex);
                    return;
                }
            }
        }
    }
    static void HandlePacket(Client client, byte[] data)
    {
        UnPacker unPacker = new();
        unPacker.SetData(data);

        if (!unPacker.Initialize()) throw new Exception("Cannot initialize unPacker.");

        handlers[(int)unPacker.protoId].HandlePacket(client, unPacker);
    }
}
