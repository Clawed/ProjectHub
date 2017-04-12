using System;
using ProjectHub.Net;

namespace ProjectHub.Core.Connections
{
    public class ConnectionManager
    {
        private readonly SocketManager Manager;

        public ConnectionManager(int Port, int MaxConnections, int ConnectionsPerIP, bool EnabeNagles)
        {
            Manager = new SocketManager();
            Manager.Init(Port, MaxConnections, ConnectionsPerIP, new InitialPacketParser(), !EnabeNagles);
        }

        public void Init()
        {
            Manager.ConnectionEvent2 += ManagerConnectionEvent;
            Manager.InitializeConnectionRequests();
        }

        private void ManagerConnectionEvent(ConnectionInformation Connection)
        {
            Connection.ConnectionChanged += ConnectionChanged;
            //ProjectHub.GetGame().GetClientManager().CreateAndStartClient(Convert.ToInt32(Connection.GetConnectionId()), Connection);
        }

        private void ConnectionChanged(ConnectionInformation Information, ConnectionState State)
        {
            if (State == ConnectionState.CLOSED)
            {
                CloseConnection(Information);
            }
        }

        private void CloseConnection(ConnectionInformation Connection)
        {
            try
            {
                Connection.Dispose();
                //ProjectHub.GetGame().GetClientManager().DisposeConnection(Convert.ToInt32(Connection.GetConnectionId()));
            }
            catch (Exception Error)
            {
                Logging.LogError(Error.ToString());
            }
        }

        public void Destroy()
        {
            Manager.Destroy();
        }
    }
}
