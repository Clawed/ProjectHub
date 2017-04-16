using System;
using ProjectHub.HabboHotel.GameClients;
using ProjectHub.Communication.Packets.Incoming;
using ProjectHub.Util;
using System.IO;
using ProjectHub.Core.Connections;

namespace ProjectHub.Net
{
    public class GamePacketParser : IDataParser
    {
        public delegate void HandlePacket(ClientPacket Message);
        private readonly GameClient CurrentClient;
        private ConnectionInformation Con;
        private bool HalfDataRecieved = false;
        private byte[] HalfData = null;
        private bool Deciphered = false;

        public GamePacketParser(GameClient Me)
        {
            CurrentClient = Me;
        }

        public void HandlePacketData(byte[] Data)
        {
            try
            {
                if (CurrentClient.RC4Client != null && !Deciphered)
                {
                    CurrentClient.RC4Client.Decrypt(ref Data);
                    Deciphered = true;
                }

                if (HalfDataRecieved)
                {
                    byte[] FullDataRcv = new byte[HalfData.Length + Data.Length];
                    Buffer.BlockCopy(HalfData, 0, FullDataRcv, 0, HalfData.Length);
                    Buffer.BlockCopy(Data, 0, FullDataRcv, HalfData.Length, Data.Length);
                    HalfDataRecieved = false;
                    HandlePacketData(FullDataRcv);

                    return;
                }

                using (BinaryReader Reader = new BinaryReader(new MemoryStream(Data)))
                {
                    if (Data.Length < 4)
                    {
                        return;
                    }

                    int MsgLen = HabboEncoding.DecodeInt32(Reader.ReadBytes(4));

                    if ((Reader.BaseStream.Length - 4) < MsgLen)
                    {
                        HalfData = Data;
                        HalfDataRecieved = true;

                        return;
                    }
                    else if (MsgLen < 0 || MsgLen > 5120)
                    {
                        return;
                    }

                    byte[] Packet = Reader.ReadBytes(MsgLen);

                    using (BinaryReader R = new BinaryReader(new MemoryStream(Packet)))
                    {
                        int Header = HabboEncoding.DecodeInt16(R.ReadBytes(2));
                        byte[] Content = new byte[Packet.Length - 2];
                        Buffer.BlockCopy(Packet, 2, Content, 0, Packet.Length - 2);
                        ClientPacket Message = new ClientPacket(Header, Content);
                        OnNewPacket.Invoke(Message);
                        Deciphered = false;
                    }

                    if (Reader.BaseStream.Length - 4 > MsgLen)
                    {
                        byte[] Extra = new byte[Reader.BaseStream.Length - Reader.BaseStream.Position];
                        Buffer.BlockCopy(Data, (int)Reader.BaseStream.Position, Extra, 0, (int)(Reader.BaseStream.Length - Reader.BaseStream.Position));
                        Deciphered = true;
                        HandlePacketData(Extra);
                    }
                }
            }
            catch (Exception e)
            {
                //log.Error("Packet Error!", e);
            }
        }

        public void Dispose()
        {
            OnNewPacket = null;
            GC.SuppressFinalize(this);
        }

        public object Clone()
        {
            return new GamePacketParser(CurrentClient);
        }

        public event HandlePacket OnNewPacket;

        public void SetConnection(ConnectionInformation _Con)
        {
            Con = _Con;
            OnNewPacket = null;
        }
    }
}