using System;
using ProjectHub.Util;

namespace ProjectHub.Communication.Encryption.Crypto.RSA
{
    public class RSAKey
    {
        public int E2 { get; private set; }
        public BigInteger E { get; private set; }
        public BigInteger N { get; private set; }
        public BigInteger D { get; private set; }
        public BigInteger P { get; private set; }
        public BigInteger Q { get; private set; }
        public BigInteger Dmp1 { get; private set; }
        public BigInteger Dmq1 { get; private set; }
        public BigInteger Coeff { get; private set; }

        protected bool CanDecrypt;
        protected bool CanEncrypt;

        public RSAKey(BigInteger _N, int _E, BigInteger _D, BigInteger _P, BigInteger _Q, BigInteger _Dmp1, BigInteger _Dmq1, BigInteger _Coeff)
        {
            E2 = _E;
            E = _E;
            N = _N;
            D = _D;
            P = _P;
            Q = _Q;
            Dmp1 = _Dmp1;
            Dmq1 = _Dmq1;
            Coeff = _Coeff;

            CanEncrypt = (N != 0 && E2 != 0);
            CanDecrypt = (CanEncrypt && D != 0);
        }

        public void GeneratePair(int B, BigInteger _E)
        {
            E = _E;
            int Qs = B >> 1;

            while (true)
            {
                while (true)
                {
                    P = BigInteger.genPseudoPrime(B - Qs, 1, new Random());

                    if ((P - 1).gcd(E) == 1 && P.isProbablePrime(10))
                    {
                        break;
                    }
                }

                while (true)
                {
                    Q = BigInteger.genPseudoPrime(Qs, 1, new Random());

                    if ((Q - 1).gcd(E) == 1 && P.isProbablePrime(10))
                    {
                        break;
                    }
                }

                if (P < Q)
                {
                    BigInteger t = P;
                    P = Q;
                    Q = t;
                }

                BigInteger phi = (P - 1) * (Q - 1);

                if (phi.gcd(E) == 1)
                {
                    N = P * Q;
                    D = E.modInverse(phi);
                    Dmp1 = D % (P - 1);
                    Dmq1 = D % (Q - 1);
                    Coeff = Q.modInverse(P);
                    break;
                }
            }

            CanEncrypt = N != 0 && E != 0;
            CanDecrypt = CanEncrypt && D != 0;
            Console.WriteLine(N.ToString(16));
            Console.WriteLine(D.ToString(16));
        }

        public static RSAKey ParsePublicKey(string N, string E)
        {
            return new RSAKey(new BigInteger(N, 16), Convert.ToInt32(E, 16), 0, 0, 0, 0, 0, 0);
        }

        public static RSAKey ParsePrivateKey(string N, string E, string D, string P = null, string Q = null, string Dmp1 = null, string Dmq1 = null, string Coeff = null)
        {
            if (P == null)
            {
                return new RSAKey(new BigInteger(N, 16), Convert.ToInt32(E, 16), new BigInteger(D, 16), 0, 0, 0, 0, 0);
            }
            else
            {
                return new RSAKey(new BigInteger(N, 16), Convert.ToInt32(E, 16), new BigInteger(D, 16), new BigInteger(P, 16), new BigInteger(Q, 16), new BigInteger(Dmp1, 16), new BigInteger(Dmq1, 16), new BigInteger(Coeff, 16));
            }
        }

        public int GetBlockSize()
        {
            return (N.bitCount() + 7) / 8;
        }

        public byte[] Encrypt(byte[] Src)
        {
            return DoEncrypt(new DoCalculateionDelegate(DoPublic), Src, Pkcs1PadType.FullByte);
        }

        public byte[] Decrypt(byte[] Src)
        {
            return DoDecrypt(new DoCalculateionDelegate(DoPublic), Src, Pkcs1PadType.FullByte);
        }

        public byte[] Sign(byte[] Src)
        {
            return DoEncrypt(new DoCalculateionDelegate(DoPrivate), Src, Pkcs1PadType.FullByte);
        }

        public byte[] Verify(byte[] Src)
        {
            return DoDecrypt(new DoCalculateionDelegate(DoPrivate), Src, Pkcs1PadType.FullByte);
        }

        private byte[] DoEncrypt(DoCalculateionDelegate Method, byte[] Src, Pkcs1PadType type)
        {
            try
            {
                int Bl = GetBlockSize();
                byte[] PaddedBytes = pkcs1pad(Src, Bl, type);
                BigInteger M = new BigInteger(PaddedBytes);

                if (M == 0)
                {
                    return null;
                }

                BigInteger C = Method(M);

                if (C == 0)
                {
                    return null;
                }

                return C.getBytes();
            }
            catch
            {
                return null;
            }
        }

        private byte[] DoDecrypt(DoCalculateionDelegate Method, byte[] Src, Pkcs1PadType Type)
        {
            try
            {
                BigInteger C = new BigInteger(Src);
                BigInteger M = Method(C);

                if (M == 0)
                {
                    return null;
                }

                int Bl = GetBlockSize();
                byte[] Bytes = pkcs1unpad(M.getBytes(), Bl, Type);

                return Bytes;
            }
            catch
            {
                return null;
            }
        }

        private byte[] pkcs1pad(byte[] Src, int N, Pkcs1PadType Type)
        {
            byte[] Bytes = new byte[N];
            int I = Src.Length - 1;

            while (I >= 0 && N > 11)
            {
                Bytes[--N] = Src[I--];
            }

            Bytes[--N] = 0;

            while (N > 2)
            {
                byte x = 0;

                switch (Type)
                {
                    case Pkcs1PadType.FullByte: x = 0xFF; break;
                    case Pkcs1PadType.RandomByte: x = Randomizer.NextByte(1, 255); break;
                }

                Bytes[--N] = x;
            }

            Bytes[--N] = (byte)Type;
            Bytes[--N] = 0;

            return Bytes;
        }

        private byte[] pkcs1unpad(byte[] Src, int N, Pkcs1PadType Type)
        {
            int I = 0;

            while (I < Src.Length && Src[I] == 0)
            {
                ++I;
            }

            if (Src.Length - I != N - 1 || Src[I] > 2)
            {
                Console.WriteLine("PKCS#1 unpad: i={0}, expected src[i]==[0,1,2], got src[i]={1}", I, Src[I].ToString("X"));

                return null;
            }

            ++I;

            while (Src[I] != 0)
            {
                if (++I >= Src.Length)
                {
                    Console.WriteLine("PKCS#1 unpad: i={0}, src[i-1]!=0 (={1})", I, Src[I - 1].ToString("X"));
                }
            }

            byte[] Bytes = new byte[Src.Length - I - 1];

            for (int P = 0; ++I < Src.Length; P++)
            {
                Bytes[P] = Src[I];
            }

            return Bytes;
        }

        protected BigInteger DoPublic(BigInteger M)
        {
            return M.modPow(E2, N);
        }

        protected BigInteger DoPrivate(BigInteger M)
        {
            if (P == 0 && Q == 0)
            {
                return M.modPow(D, N);
            }
            else
            {
                return 0;
            }
        }
    }

    public delegate BigInteger DoCalculateionDelegate(BigInteger M);

    public enum Pkcs1PadType
    {
        FullByte = 1,
        RandomByte = 2
    }
}
