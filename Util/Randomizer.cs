using System;

namespace ProjectHub.Util
{
    public class Randomizer
    {
        private static Random Rand = new Random();

        public static Random GetRandom
        {
            get
            {
                return Rand;
            }
        }

        public static int Next()
        {
            return Rand.Next();
        }

        public static int Next(int Max)
        {
            return Rand.Next(Max);
        }

        public static int Next(int Min, int Max)
        {
            return Rand.Next(Min, Max);
        }

        public static double NextDouble()
        {
            return Rand.NextDouble();
        }

        public static byte NextByte()
        {
            return (byte)Next(0, 255);
        }

        public static byte NextByte(int Max)
        {
            Max = Math.Min(Max, 255);
            return (byte)Next(0, Max);
        }

        public static byte NextByte(int Min, int Max)
        {
            Max = Math.Min(Max, 255);
            return (byte)Next(Math.Min(Min, Max), Max);
        }

        public static void NextBytes(byte[] ToParse)
        {
            Rand.NextBytes(ToParse);
        }
    }
}
