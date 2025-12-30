namespace T.Integration.Protocol.ServerProtocol.ServerPackets;

internal class KickClientCmd : IServerPacket
{
    public ulong client;

    public byte[] Pack()
    {
        Packer p = new();
        p.PushUInt64(client);

        return p.Pack(Cmd.KickClient);
    }
}
