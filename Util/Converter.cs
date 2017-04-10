using System;

namespace ProjectHub.Util
{
    public class Converter
    {
        public static string BytesToHexString(byte[] Bytes)
        {
            string HexString = BitConverter.ToString(Bytes);

            return HexString.Replace("-", "");
        }

        public static byte[] HexStringToBytes(string HexString)
        {
            int NumberChars = HexString.Length;
            byte[] Bytes = new byte[NumberChars / 2];

            for (int I = 0; I < NumberChars; I += 2)
            {
                Bytes[I / 2] = Convert.ToByte(HexString.Substring(I, 2), 16);
            }

            return Bytes;
        }
    }
}
