namespace T.Integration;

internal enum DisconnectCode
{
    SocketDisconnect = 0,
    DataOutOfBounds = 1,
    ReadException = 2,
    PacketException = 3,
    NotIdentified = 4,
    VersionMismatch = 5
}