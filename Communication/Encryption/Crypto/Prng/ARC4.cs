namespace ProjectHub.Communication.Encryption.Crypto.Prng
{
    public class ARC4
    {
        private int I;
        private int J;
        private byte[] Bytes;

        public const int POOLSIZE = 256;

        public ARC4()
        {
            Bytes = new byte[POOLSIZE];
        }

        public ARC4(byte[] Key)
        {
            Bytes = new byte[POOLSIZE];
            Initialize(Key);
        }

        public void Initialize(byte[] Key)
        {
            I = 0;
            J = 0;

            for (I = 0; I < POOLSIZE; ++I)
            {
                Bytes[I] = (byte)I;
            }

            for (I = 0; I < POOLSIZE; ++I)
            {
                J = (J + Bytes[I] + Key[I % Key.Length]) & (POOLSIZE - 1);
                Swap(I, J);
            }

            I = 0;
            J = 0;
        }

        private void Swap(int A, int B)
        {
            byte T = Bytes[A];
            Bytes[A] = Bytes[B];
            Bytes[B] = T;
        }

        public byte Next()
        {
            I = ++I & (POOLSIZE - 1);
            J = (J + Bytes[I]) & (POOLSIZE - 1);
            Swap(I, J);

            return Bytes[(Bytes[I] + Bytes[J]) & 255];
        }

        public void Encrypt(ref byte[] Src)
        {
            for (int K = 0; K < Src.Length; K++)
            {
                Src[K] ^= Next();
            }
        }

        public void Decrypt(ref byte[] Src)
        {
            Encrypt(ref Src);
        }
    }
}
