using System;
using System.Collections.Generic;
using System.Text;
using ProjectHub.Communication.Interfaces;

namespace ProjectHub.Communication.Packets.Outgoing
{
    public class ServerPacket : IServerPacket
    {
        private readonly Encoding Encoding = Encoding.Default;
        private List<byte> Body = new List<byte>();

        public ServerPacket(int id)
        {
            Id = id;
            WriteShort(id);
        }

        public int Id { get; private set; }

        public byte[] GetBytes()
        {
            var Final = new List<byte>();
            Final.AddRange(BitConverter.GetBytes(Body.Count));
            Final.Reverse();
            Final.AddRange(Body);

            return Final.ToArray();
        }

        public void WriteByte(byte B)
        {
            Body.Add(B);
        }

        public void WriteByte(int B)
        {
            Body.Add((byte)B);
        }

        public void WriteBytes(byte[] B, bool IsInt)
        {
            if (IsInt)
            {
                for (int I = (B.Length - 1); I > -1; I--)
                {
                    Body.Add(B[I]);
                }
            }
            else
            {
                Body.AddRange(B);
            }
        }

        public void WriteDouble(double D)
        {
            string Raw = Math.Round(D, 1).ToString();

            if (Raw.Length == 1)
            {
                Raw += ".0";
            }

            WriteString(Raw.Replace(',', '.'));
        }

        public void WriteString(string S)
        {
            WriteShort(S.Length);
            WriteBytes(Encoding.GetBytes(S), false);
        }

        public void WriteShort(int S)
        {
            var I = (Int16) S;
            WriteBytes(BitConverter.GetBytes(I), true);
        }

        public void WriteInteger(int I)
        {
            WriteBytes(BitConverter.GetBytes(I), true);
        }

        public void WriteBoolean(bool B)
        {
            WriteBytes(new[] {(byte) (B ? 1 : 0)}, false);
        }
    }
}