using T.Integration.Binary;
using T.Integration.Helpers;

namespace T.Integration.Protocol;

internal class Packer : BufferWriter
{
    public ushort reserve;

    public byte[] Pack(SystemProtocol.Cmd cmd) => Pack(ProtoID.System, (ushort)cmd);
    public byte[] Pack(ServerProtocol.Cmd cmd) => Pack(ProtoID.Server, (ushort)cmd);

    public byte[] Pack(ProtoID protoId, ushort cmd)
    {
        byte[] data = EncryptionHelper.EncryptData(ToByteArray());

        m_data = [];

        PushUInt16((ushort)protoId);
        PushUInt16(cmd);
        PushUInt16((ushort)data.Length);
        PushUInt16(reserve);

        EncryptionHelper.EncryptHeader(m_data);
        m_data.AddRange(data);

        return ToByteArray();
    }
}
