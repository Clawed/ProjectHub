using System;
using System.Net.Sockets;
using System.Text;

namespace ProjectHub.Core.Connections
{
    public class ConnectionInformation : IDisposable
    {
        public delegate void ConnectionChange(ConnectionInformation Information, ConnectionState State);
        private static bool DisableSend = false;
        private static bool DisableReceive = false;
        private readonly byte[] Buffer;
        private readonly int ConnectionId;
        private readonly Socket DataSocket;
        private readonly string Ip;
        private readonly AsyncCallback SendCallback;
        private bool IsConnected;
        private SocketManager Manager;
        public IDataParser Parser { get; set; }
        public event ConnectionChange ConnectionChanged;

        public ConnectionInformation(int _ConnectionId, Socket DataStream, SocketManager _Manager, IDataParser _Parser, string _Ip)
        {
            Parser = _Parser;
            Buffer = new byte[SocketManagerStatics.BUFFER_SIZE];
            Manager = _Manager;
            DataSocket = DataStream;
            DataSocket.SendBufferSize = SocketManagerStatics.BUFFER_SIZE;
            Ip = _Ip;
            SendCallback = SentData;
            ConnectionId = _ConnectionId;

            if (ConnectionChanged != null)
            {
                ConnectionChanged.Invoke(this, ConnectionState.OPEN);
            }
        }

        public void StartPacketProcessing()
        {
            if (!IsConnected)
            {
                IsConnected = true;

                try
                {
                    DataSocket.BeginReceive(Buffer, 0, Buffer.Length, SocketFlags.None, IncomingDataPacket, DataSocket);
                }
                catch
                {
                    Disconnect();
                }
            }
        }

        public string GetIp()
        {
            return Ip;
        }

        public int GetConnectionId()
        {
            return ConnectionId;
        }

        public void Dispose()
        {
            if (IsConnected)
            {
                Disconnect();
            }

            GC.SuppressFinalize(this);
        }

        public void Disconnect()
        {
            try
            {
                if (IsConnected)
                {
                    IsConnected = false;

                    try
                    {
                        if (DataSocket != null && DataSocket.Connected)
                        {
                            DataSocket.Shutdown(SocketShutdown.Both);
                            DataSocket.Close();
                        }
                    }
                    catch
                    {
                    }

                    DataSocket.Dispose();
                    Parser.Dispose();

                    try
                    {
                        if (ConnectionChanged != null)
                        {
                            ConnectionChanged.Invoke(this, ConnectionState.CLOSED);
                        }
                    }
                    catch
                    {
                    }

                    ConnectionChanged = null;
                }
            }
            catch
            {
            }
        }

        private void IncomingDataPacket(IAsyncResult Async)
        {
            int BytesReceived;

            try
            {
                BytesReceived = DataSocket.EndReceive(Async);
            }
            catch
            {
                Disconnect();

                return;
            }

            if (BytesReceived == 0)
            {
                Disconnect();

                return;
            }

            try
            {
                if (!DisableReceive)
                {
                    var packet = new byte[BytesReceived];
                    Array.Copy(Buffer, packet, BytesReceived);
                    HandlePacketData(packet);
                }
            }
            catch //(Exception e)
            {
                Disconnect();
            }
            finally
            {
                try
                {
                    DataSocket.BeginReceive(Buffer, 0, Buffer.Length, SocketFlags.None, IncomingDataPacket, DataSocket);
                }
                catch
                {
                    Disconnect();
                }
            }
        }

        private void HandlePacketData(byte[] Packet)
        {
            if (Parser != null)
            {
                Parser.HandlePacketData(Packet);
            }
        }

        public void SendData(byte[] Packet)
        {
            try
            {
                if (!IsConnected || DisableSend)
                {
                    return;
                }

                string PacketData = Encoding.Default.GetString(Packet);
                DataSocket.BeginSend(Packet, 0, Packet.Length, 0, SendCallback, null);
            }
            catch
            {
                Disconnect();
            }
        }

        private void SentData(IAsyncResult Async)
        {
            try
            {
                DataSocket.EndSend(Async);
            }
            catch
            {
                Disconnect();
            }
        }
    }
}