namespace T.Integration.Protocol;

internal abstract class ProtocolHandler
{
    public abstract void HandlePacket(Client client, UnPacker unPacker);
}
