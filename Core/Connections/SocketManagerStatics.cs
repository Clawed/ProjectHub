﻿namespace ProjectHub.Core.Connections
{
    public class SocketManagerStatics
    {
        public static readonly int BUFFER_SIZE = 8192;
        public static readonly int MAX_PACKET_SIZE = BUFFER_SIZE - 4;
    }
}