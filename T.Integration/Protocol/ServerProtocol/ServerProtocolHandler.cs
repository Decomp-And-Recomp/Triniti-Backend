namespace T.Integration.Protocol.ServerProtocol;

internal class ServerProtocolHandler : ProtocolHandler
{
    public override void HandlePacket(Client client, UnPacker unPacker)
    {
        switch ((Cmd)unPacker.cmd)
        {
            //case Cmd.Heartbeat: OnIdentify(client, unPacker); return;
            default: throw new NotImplementedException($"{unPacker.protoId}:{unPacker.cmd}");
        }
    }

    static void OnIdentify(Client client, UnPacker unPacker)
    {
        //a cmd = new();

        if (!cmd.Parse(unPacker)) throw new Exception("Cant parse 'IdentifyCmd'");


        client.identifyData = cmd;
    }
}
