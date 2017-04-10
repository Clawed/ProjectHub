namespace ProjectHub.Util
{
    public class TextHandling
    {
        public static int Parse(string A)
        {
            int W = 0, I = 0, Length = A.Length, K;

            if (Length == 0)
                return 0;

            do
            {
                K = A[I++];
                if (K < 48 || K > 59)
                    return 0;
                W = 10*W + K - 48;
            } while (I < Length);

            return W;
        }

        public static string GetString(double K)
        {
            return K.ToString(ProjectHub.CultureInfo);
        }
    }
}