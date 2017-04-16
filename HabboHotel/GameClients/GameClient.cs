using ProjectHub.Communication.Encryption.Crypto.Prng;
using ProjectHub.Communication.Interfaces;
using ProjectHub.Communication.Packets.Incoming;
using ProjectHub.Core;
using ProjectHub.Core.Connections;
using ProjectHub.Database.Interfaces;
using ProjectHub.HabboHotel.Users;
using ProjectHub.Net;
using System;

namespace ProjectHub.HabboHotel.GameClients
{
    public class GameClient
    {
        public ARC4 RC4Client = null;

        private readonly int Id;
        private Habbo Habbo;
        public string MachineId;
        private bool Disconnected;

        private GamePacketParser PacketParser;
        private ConnectionInformation Connection;
        public int PingCount { get; set; }

        public GameClient(int ClientId, ConnectionInformation _Connection)
        {
            Id = ClientId;
            Connection = _Connection;
            PacketParser = new GamePacketParser(this);
            PingCount = 0;
        }

        private void SwitchParserRequest()
        {
            PacketParser.SetConnection(Connection);
            PacketParser.OnNewPacket += ParserOnNewPacket;
            byte[] data = (Connection.Parser as InitialPacketParser).CurrentData;
            Connection.Parser.Dispose();
            Connection.Parser = PacketParser;
            Connection.Parser.HandlePacketData(data);
        }

        private void ParserOnNewPacket(ClientPacket Message)
        {
            try
            {
                ProjectHub.GetGame().GetPacketManager().TryExecutePacket(this, Message);
            }
            catch (Exception Error)
            {
                Logging.LogError(Error.ToString());
            }
        }

        private void PolicyRequest()
        {
            Connection.SendData(ProjectHub.GetDefaultEncoding().GetBytes("<?xml version=\"1.0\"?>\r\n" +
                   "<!DOCTYPE cross-domain-policy SYSTEM \"/xml/dtds/cross-domain-policy.dtd\">\r\n" +
                   "<cross-domain-policy>\r\n" +
                   "<allow-access-from domain=\"*\" to-ports=\"1-31111\" />\r\n" +
                   "</cross-domain-policy>\x0"));
        }

        public void StartConnection()
        {
            if (Connection == null)
            {
                return;
            }

            PingCount = 0;
            (Connection.Parser as InitialPacketParser).PolicyRequest += PolicyRequest;
            (Connection.Parser as InitialPacketParser).SwitchParserRequest += SwitchParserRequest;
            Connection.StartPacketProcessing();
        }

        public bool TryAuthenticate(string AuthTicket)
        {
            // Todo
        }

        public void SendWhisper(string Message, int Colour = 0)
        {
            // Todo
        }

        public void SendNotification(string Message)
        {
            // Todo
        }

        public void SendMessage(IServerPacket Message)
        {
            byte[] Bytes = Message.GetBytes();

            if (Message == null)
            {
                return;
            }

            if (GetConnection() == null)
            {
                return;
            }

            GetConnection().SendData(Bytes);
        }

        public int ConnectionId
        {
            get { return Id; }
        }

        public ConnectionInformation GetConnection()
        {
            return Connection;
        }

        public Habbo GetHabbo()
        {
            return Habbo;
        }

        public void Disconnect()
        {
            try
            {
                if (GetHabbo() != null)
                {
                    using (IQueryAdapter DbClient = ProjectHub.GetDatabaseManager().GetQueryReactor())
                    {
                        DbClient.RunQuery(GetHabbo().GetQueryString);
                    }

                    GetHabbo().OnDisconnect();
                }
            }
            catch (Exception Error)
            {
                Logging.LogError(Error.ToString());
            }


            if (!Disconnected)
            {
                if (Connection != null)
                {
                    Connection.Dispose();
                }

                Disconnected = true;
            }
        }

        public void Dispose()
        {
            if (GetHabbo() != null)
            {
                GetHabbo().OnDisconnect();
            }

            MachineId = string.Empty;
            Disconnected = true;
            Habbo = null;
            Connection = null;
            RC4Client = null;
            PacketParser = null;
        }
    }
}