using ProjectHub.Core;
using System;
using System.Net.Sockets;
using System.Text;

namespace ProjectHub.Net.Mus
{

    public class MusConnection
    {
        private Socket Socket;
        private byte[] Buffer = new byte[1024];

        public MusConnection(Socket _Socket)
        {
            Socket = _Socket;

            try
            {
                Socket.BeginReceive(Buffer, 0, Buffer.Length, SocketFlags.None, OnEvent_RecieveData, Socket);
            }
            catch
            {
                TryClose();
            }
        }

        public void TryClose()
        {
            try
            {
                Socket.Shutdown(SocketShutdown.Both);
                Socket.Close();
                Socket.Dispose();
            }
            catch
            {
            }

            Socket = null;
        }

        public void OnEvent_RecieveData(IAsyncResult Async)
        {
            try
            {
                int Bytes = 0;

                try
                {
                    Bytes = Socket.EndReceive(Async);
                }
                catch
                {
                    TryClose();
                    return;
                }

                String Data = Encoding.Default.GetString(Buffer, 0, Bytes);

                if (Data.Length > 0)
                {
                    ProcessCommand(Data);
                }
            }
            catch (Exception Error)
            {
                Logging.LogError(Error.ToString());
            }

            TryClose();
        }

        public void ProcessCommand(String Data)
        {
            String Header = Data.Split(Convert.ToChar(1))[0];
            String Param = Data.Split(Convert.ToChar(1))[1];

            string[] Params = Param.ToString().Split(':');

            switch (Header.ToLower())
            {
                case "update_settings":
                case "reload_settings":
                    {
                        ProjectHub.SettingsData = new SettingsData();
                        new Logger("mus", Header, Param);
                        break;
                    }
                case "update_texts":
                case "reload_texts":
                    {
                        ProjectHub.TextsData = new TextsData();
                        new Logger("mus", Header, Param);
                        break;
                    }
                case "give_credits":
                    {
                        new Logger("mus", Header, Param);
                        break;
                    }

                default:
                    {
                        Logging.WriteLine("MUS command doesn't exist: '" + Header + "'");
                        return;
                    }
            }

            Logging.WriteLine("MUS command executed: '" + Header + "'");
        }
    }
}