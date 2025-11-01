using T.Integration.Binary;

namespace T.Integration.Protocol;

internal interface IClientPacket
{
    public bool Parse(BufferReader reader);
}
