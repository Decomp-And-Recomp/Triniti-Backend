using T.Integration.Binary;
using T.Integration.Helpers;

namespace T.Integration.Protocol;

internal class UnPacker : BufferReader
{
    public ProtoID protoId { get; private set; }

    ushort _cmd;
    public ushort cmd => _cmd;

    ushort _length;
    public ushort length => _length;

    ushort _reserve;
    public ushort reserve => _reserve;

    public bool Initialize()
    {
        ushort p = 0;

        if (!PopUInt16(ref p)) return false;
        if (!PopUInt16(ref _cmd)) return false;
        if (!PopUInt16(ref _length)) return false;
        if (!PopUInt16(ref _reserve)) return false;

        byte[] data = new byte[_length];
        if (!PopByteArray(ref data)) return false;

        SetData(EncryptionHelper.DecryptData(data));
        protoId = (ProtoID)p;
        return true;
    }
}
