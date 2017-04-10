using ProjectHub.Core;
using System;

namespace ProjectHub.Util
{
    public class HabboEncoding
    {
        public static string EncodeInt32(int V)
        {
            string T = "";

            return ((T + ((char)(V >> 0x18)) + ((char)(V >> 0x10))) + ((char)(V >> 8)) + ((char)V));
        }

        public static string EncodeInt16(int V)
        {
            string T = "";

            return (T + ((char)(V >> 8)) + ((char)V));
        }

        public static int DecodeInt32(string V)
        {
            if ((((V[0] | V[1]) | V[2]) | V[3]) < 0)
            {
                return -1;
            }

            return ((((V[0] << 0x18) + (V[1] << 0x10)) + (V[2] << 8)) + V[3]);
        }

        public static int DecodeInt32(byte[] V)
        {
            if ((((V[0] | V[1]) | V[2]) | V[3]) < 0)
            {
                return -1;
            }

            return ((((V[0] << 0x18) + (V[1] << 0x10)) + (V[2] << 8)) + V[3]);
        }

        public static int DecodeInt16(byte[] V)
        {
            if ((V[0] | V[1]) < 0)
            {
                return -1;
            }

            return ((V[0] << 8) + V[1]);
        }

        public static short DecodeInt16(string V)
        {
            if ((V[0] | V[1]) < 0)
            {
                return -1;
            }

            return (short)((V[0] << 8) + V[1]);
        }

        public static bool DecodeBool(string V)
        {
            try
            {
                int I = Convert.ToInt32(Convert.ToChar(V.Substring(0, 1)));

                return (I == 1);
            }
            catch (Exception Error)
            {
                Logging.WriteLine(Error.ToString());

                return false;
            }
        }
    }
}
