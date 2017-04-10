using System;

namespace ProjectHub.Util
{
    public static class RandomNumber
    {
        private static readonly Random R = new Random();
        private static readonly Object L = new Object();

        private static readonly Random GlobalRandom = new Random();
        [ThreadStatic] private static Random LocalRandom;

        public static int GenerateNewRandom(int Min, int Max)
        {
            return new Random().Next(Min, Max + 1);
        }

        public static int GenerateLockedRandom(int Min, int Max)
        {
            lock (L)
            {
                return R.Next(Min, Max);
            }
        }

        public static int GenerateRandom(int Min, int Max)
        {
            Random Inst = LocalRandom;

            Max++;

            if (Inst == null)
            {
                int Seed;

                lock (GlobalRandom)
                {
                    Seed = GlobalRandom.Next();
                }

                LocalRandom = Inst = new Random(Seed);
            }

            return Inst.Next(Min, Max);
        }
    }
}