namespace T.Integration.Protocol.ServerProtocol;

internal enum Cmd : ushort
{
    KickClient = 0,
    KickClientRange = 1,
    ShutRoom = 2,
    ShutRoomRange = 3,
    ListClients = 4,
    ListRooms = 5,
    ListClientsInRoom = 6,
    IsIpBanned = 7
}
