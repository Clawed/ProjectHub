using System.Text;
using ProjectHub.Util;
using ProjectHub.Communication.Encryption.Keys;
using ProjectHub.Communication.Encryption.Crypto.RSA;
using ProjectHub.Communication.Encryption.KeyExchange;

namespace ProjectHub.Communication.Encryption
{
    public static class HabboEncryptionV2
    {
        private static RSAKey Rsa;
        private static DiffieHellman DiffieHellman;

        public static void Initialize(RSAKeys Keys)
        {
            Rsa = RSAKey.ParsePrivateKey(Keys.N, Keys.E, Keys.D);
            DiffieHellman = new DiffieHellman();
        }

        private static string GetRsaStringEncrypted(string Message)
        {
            try
            {
                byte[] M = Encoding.Default.GetBytes(Message);
                byte[] C = Rsa.Sign(M);

                return Converter.BytesToHexString(C);
            }
            catch
            {
                return "0";
            }
        }

        public static string GetRsaDiffieHellmanPrimeKey()
        {
            string Key = DiffieHellman.Prime.ToString(10);

            return GetRsaStringEncrypted(Key);
        }

        public static string GetRsaDiffieHellmanGeneratorKey()
        {
            string Key = DiffieHellman.Generator.ToString(10);

            return GetRsaStringEncrypted(Key);
        }

        public static string GetRsaDiffieHellmanPublicKey()
        {
            string Key = DiffieHellman.PublicKey.ToString(10);

            return GetRsaStringEncrypted(Key);
        }

        public static BigInteger CalculateDiffieHellmanSharedKey(string PublicKey)
        {
            try
            {
                byte[] Bytes = Converter.HexStringToBytes(PublicKey);
                byte[] PublicKeyBytes = Rsa.Verify(Bytes);
                string PublicKeyString = Encoding.Default.GetString(PublicKeyBytes);

                return DiffieHellman.CalculateSharedKey(new BigInteger(PublicKeyString, 10));
            }
            catch
            {
                return 0;
            }
        }
    }
}
