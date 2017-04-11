using System;
using ProjectHub.Util;

namespace ProjectHub.Communication.Encryption.KeyExchange
{
    public class DiffieHellman
    {
        public readonly int BITLENGTH = 32;
        public BigInteger Prime { get; private set; }
        public BigInteger Generator { get; private set; }
        private BigInteger PrivateKey;
        public BigInteger PublicKey { get; private set; }

        public DiffieHellman()
        {
            Initialize();
        }

        public DiffieHellman(int B)
        {
            BITLENGTH = B;

            Initialize();
        }

        public DiffieHellman(BigInteger _Prime, BigInteger _Generator)
        {
            Prime = _Prime;
            Generator = _Generator;

            Initialize(true);
        }

        private void Initialize(bool IgnoreBaseKeys = false)
        {
            PublicKey = 0;
            Random Rand = new Random();

            while (PublicKey == 0)
            {
                if (!IgnoreBaseKeys)
                {
                    Prime = BigInteger.genPseudoPrime(BITLENGTH, 10, Rand);
                    Generator = BigInteger.genPseudoPrime(BITLENGTH, 10, Rand);
                }

                byte[] bytes = new byte[BITLENGTH / 8];
                Randomizer.NextBytes(bytes);
                PrivateKey = new BigInteger(bytes);

                if (Generator > Prime)
                {
                    BigInteger temp = Prime;
                    Prime = Generator;
                    Generator = temp;
                }

                PublicKey = Generator.modPow(PrivateKey, Prime);

                if (!IgnoreBaseKeys)
                {
                    break;
                }
            }
        }

        public BigInteger CalculateSharedKey(BigInteger M)
        {
            return M.modPow(PrivateKey, Prime);
        }
    }
}

