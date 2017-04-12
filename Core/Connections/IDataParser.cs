using System;

namespace ProjectHub.Core.Connections
{
    public interface IDataParser : IDisposable, ICloneable
    {
        void HandlePacketData(byte[] Packet);
    }
}