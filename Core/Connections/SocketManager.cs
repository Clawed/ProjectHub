using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;

namespace ProjectHub.Core.Connections
{
    public class SocketManager
    {
        public delegate void ConnectionEvent(ConnectionInformation connection);
        private bool AcceptConnections;
        private int AcceptedConnections;
        private Socket ConnectionListener;
        private bool DisableNaglesAlgorithm;
        private int MaxIpConnectionCount;
        private int MaxConnections;
        private IDataParser Parser;
        private int PortId;
        public event ConnectionEvent ConnectionEvent2;
        private ConcurrentDictionary<string, int> IpConnectionsCount;

        public void Init(int _PortId, int _MaxConnections, int _ConnectionsPerIp, IDataParser _Parser, bool _DisableNaglesAlgorithm)
        {
            IpConnectionsCount = new ConcurrentDictionary<string, int>();

            Parser = _Parser;
            DisableNaglesAlgorithm = _DisableNaglesAlgorithm;
            MaxConnections = _MaxConnections;
            PortId = _PortId;
            MaxIpConnectionCount = _ConnectionsPerIp;
            PrepareConnectionDetails();
            AcceptedConnections = 0;
        }

        private void PrepareConnectionDetails()
        {
            ConnectionListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ConnectionListener.NoDelay = DisableNaglesAlgorithm;

            try
            {
                ConnectionListener.Bind(new IPEndPoint(IPAddress.Any, PortId));
            }
            catch (SocketException Error)
            {
                throw new Exception(Error.Message);
            }
        }

        public void InitializeConnectionRequests()
        {
            ConnectionListener.Listen(100);
            AcceptConnections = true;

            try
            {
                ConnectionListener.BeginAccept(NewConnectionRequest, ConnectionListener);
            }
            catch
            {
                Destroy();
            }
        }

        public void Destroy()
        {
            AcceptConnections = false;

            try
            {
                ConnectionListener.Close();
            }
            catch
            {
            }

            ConnectionListener = null;
        }

        private void NewConnectionRequest(IAsyncResult iAr)
        {
            if (ConnectionListener != null)
            {
                if (AcceptConnections)
                {
                    try
                    {
                        Socket replyFromComputer = ((Socket)iAr.AsyncState).EndAccept(iAr);
                        replyFromComputer.NoDelay = DisableNaglesAlgorithm;
                        string Ip = replyFromComputer.RemoteEndPoint.ToString().Split(':')[0];
                        int ConnectionCount = GetAmountOfConnectionFromIp(Ip);

                        if (ConnectionCount < MaxIpConnectionCount)
                        {
                            AcceptedConnections++;
                            ConnectionInformation C = new ConnectionInformation(AcceptedConnections, replyFromComputer, this, Parser.Clone() as IDataParser, Ip);
                            ReportUserLogin(Ip);
                            C.ConnectionChanged += CConnectionChanged;

                            if (ConnectionEvent2 != null)
                            {
                                ConnectionEvent2(C);
                            }
                        }
                        else
                        {
                            //log.Info("Connection denied from [" + replyFromComputer.RemoteEndPoint.ToString().Split(':')[0] + "]. Too many connections (" + ConnectionCount + ").");
                        }
                    }
                    catch
                    {
                    }
                    finally
                    {
                        ConnectionListener.BeginAccept(NewConnectionRequest, ConnectionListener);
                    }
                }
                else
                {
                }
            }
        }

        private void CConnectionChanged(ConnectionInformation Information, ConnectionState State)
        {
            if (State == ConnectionState.CLOSED)
            {
                ReportDisconnect(Information);
            }
        }

        public void ReportDisconnect(ConnectionInformation GameConnection)
        {
            GameConnection.ConnectionChanged -= CConnectionChanged;
            ReportUserLogout(GameConnection.GetIp());
           
        }

        private void ReportUserLogin(string ip)
        {
            AlterIpConnectionCount(ip, (GetAmountOfConnectionFromIp(ip) + 1));
        }

        private void ReportUserLogout(string ip)
        {
            AlterIpConnectionCount(ip, (GetAmountOfConnectionFromIp(ip) - 1));
        }

        private void AlterIpConnectionCount(string ip, int amount)
        {
            if (IpConnectionsCount.ContainsKey(ip))
            {
                int am;
                IpConnectionsCount.TryRemove(ip, out am);
            }
            IpConnectionsCount.TryAdd(ip, amount);
        }

        private int GetAmountOfConnectionFromIp(string ip)
        {
            if (IpConnectionsCount.ContainsKey(ip))
            {
                return IpConnectionsCount[ip];
            }
            else
            {
                return 0;
            }
        }
    }
}