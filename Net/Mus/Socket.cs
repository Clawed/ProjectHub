using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

namespace ProjectHub.Net.Mus
{
    public class MusSocket
    {
        private Socket MusSocket2;
        private List<String> AllowedIPs;

        private String MusIP;
        private int MusPort;

        public MusSocket(String _MusIP, int _MusPort, String[] _AllowdIPs, int _Backlog)
        {
            MusIP = _MusIP;
            MusPort = _MusPort;

            AllowedIPs = new List<String>();

            foreach (String Ip in _AllowdIPs)
            {
                AllowedIPs.Add(Ip);
            }

            MusSocket2 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            MusSocket2.Bind(new IPEndPoint(IPAddress.Any, MusPort));
            MusSocket2.Listen(_Backlog);
            MusSocket2.BeginAccept(OnEvent_NewConnection, MusSocket2);
        }

        private void OnEvent_NewConnection(IAsyncResult Async)
        {
            try
            {
                Socket Socket = ((Socket)Async.AsyncState).EndAccept(Async);
                String ip = Socket.RemoteEndPoint.ToString().Split(':')[0];
                if (AllowedIPs.Contains(ip) || ip == "127.0.0.1")
                {
                    var NC = new MusConnection(Socket);
                }
                else
                {
                    Socket.Close();
                }
            }
            catch (Exception)
            {
            }

            MusSocket2.BeginAccept(OnEvent_NewConnection, MusSocket2);
        }
    }
}