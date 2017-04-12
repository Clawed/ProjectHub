using ProjectHub.HabboHotel.GameClients;

namespace ProjectHub.Communication.Packets.Incoming.Handshake
{
    public class GetClientVersionEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            string Build = Packet.PopString();

            if (ProjectHub.SWFRevision != Build)
            {
                ProjectHub.SWFRevision = Build;
            }
        }
    }
}