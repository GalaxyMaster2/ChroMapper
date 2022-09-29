using System.Collections.Generic;
using System.Net;
using LiteNetLib;
using LiteNetLib.Utils;

// When using ChroMapTogether, the session host is effectively another client of the ChroMapTogether server.
public class MultiServerRelayModeNetListener : MultiClientNetListener
{
    private AutoSaveController autoSave;
    private List<string> tempBannedIps = new List<string>();

    public MultiServerRelayModeNetListener(string roomCode, MapperIdentityPacket identity, AutoSaveController autoSave)
        : base(roomCode, identity)
    {
        this.autoSave = autoSave;

        SubscribeToCollectionEvents();

        RegisterPacketHandler(PacketId.CMT_RequestZip, OnRequestZip);
        RegisterPacketHandler(PacketId.CMT_IncomingMapper, OnIncomingMapper);
    }

    public override void Dispose()
    {
        UnsubscribeFromCollectionEvents();
        base.Dispose();
    }

    // This is absolutely NOT a good way to go about this, but I can't think of anything else!
    //   At least this time, we are only doing it when ChroMapTogether requests a zip
    public void OnRequestZip(MultiNetListener _, MapperIdentityPacket identity, NetDataReader reader)
        => PersistentUI.Instance.StartCoroutine(MultiServerNetListener.SaveAndSendMapToPeer(this, autoSave, NetManager.FirstPeer));

    public void OnIncomingMapper(MultiNetListener listener, MapperIdentityPacket identity, NetDataReader reader)
    {
        var ip = reader.GetString();

        var writer = new NetDataWriter();

        writer.Put(identity.ConnectionId);

        if (!IPAddress.TryParse(ip, out _))
        {
            writer.Put("IP Address was invalid.");
            SendPacketTo(NetManager.FirstPeer, PacketId.CMT_KickMapper, writer.Data);
            return;
        }
        else if (tempBannedIps.Contains(ip))
        {
            writer.Put("The host has banned you.");
            SendPacketTo(NetManager.FirstPeer, PacketId.CMT_KickMapper, writer.Data);
            return;
        }

        SendPacketTo(NetManager.FirstPeer, PacketId.CMT_AcceptMapper, writer.Data);
    }

    // No longer doing anything since latency is updated completely via MapperLatencyPackets
    public override void OnNetworkLatencyUpdate(NetPeer peer, int latency) { }
    
    // The ChroMapTogether server lost connection. As the host, don't return to song select.
    public override void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        PersistentUI.Instance.ShowDialogBox("MultiMapping", "multi.connection.server-lost", null,
            PersistentUI.DialogBoxPresetType.Ok, new object[] { disconnectInfo.Reason });
    }
}
