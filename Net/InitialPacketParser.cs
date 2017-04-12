using ProjectHub.Core.Connections;
using System;

namespace ProjectHub.Net
{
    public class InitialPacketParser : IDataParser
    {
        public delegate void NoParamDelegate();

        public byte[] CurrentData;

        public void HandlePacketData(byte[] Packet)
        {
            if (Packet[0] == 60 && PolicyRequest != null)
            {
                PolicyRequest.Invoke();
            }
            else if (Packet[0] != 67 && SwitchParserRequest != null)
            {
                CurrentData = Packet;
                SwitchParserRequest.Invoke();
            }
        }

        public void Dispose()
        {
            PolicyRequest = null;
            SwitchParserRequest = null;
            GC.SuppressFinalize(this);
        }

        public object Clone()
        {
            return new InitialPacketParser();
        }

        public event NoParamDelegate PolicyRequest;
        public event NoParamDelegate SwitchParserRequest;
    }
}