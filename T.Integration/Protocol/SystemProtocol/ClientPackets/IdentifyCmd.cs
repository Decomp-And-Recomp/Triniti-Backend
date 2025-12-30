using T.Integration.Binary;

namespace T.Integration.Protocol.SystemProtocol.ClientPackets;

internal class IdentifyCmd : IClientPacket
{
    public string name = string.Empty;
    public ulong apiVer;

    public bool Parse(BufferReader reader)
    {
        if (!reader.PopString(ref name)) return false;
        if (!reader.PopUInt64(ref apiVer)) return false;

        return true;
    }
}
