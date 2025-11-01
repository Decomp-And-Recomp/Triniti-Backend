using T.Integration.Protocol.SystemProtocol.ClientPackets;

namespace T.Integration.Protocol.SystemProtocol;

internal class SystemProtocolHandler : ProtocolHandler
{
    public override void HandlePacket(Client client, UnPacker unPacker)
    {
        switch ((Cmd)unPacker.cmd)
        {
            case Cmd.Identify: OnIdentify(client, unPacker); return;
            //case Cmd.Heartbeat: OnIdentify(client, unPacker); return;
            default: throw new NotImplementedException($"{unPacker.protoId}:{unPacker.cmd}");
        }
    }

    static void OnIdentify(Client client, UnPacker unPacker)
    {
        IdentifyCmd cmd = new();

        if (!cmd.Parse(unPacker)) throw new Exception("Cant parse 'IdentifyCmd'");

        if (cmd.apiVer != Consts.apiVer)
        {
            client.Disconnect(DisconnectCode.VersionMismatch);
            return;
        }

        client.identifyData = cmd;
    }
}
