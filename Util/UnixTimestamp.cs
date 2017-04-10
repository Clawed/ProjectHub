using System;

namespace ProjectHub.Util
{
    static class UnixTimestamp
    {
        public static double GetNow()
        {
            TimeSpan Ts = (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0));
            return Ts.TotalSeconds;
        }

        public static DateTime FromUnixTimestamp(double Timestamp)
        {
            DateTime Dt = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            Dt = Dt.AddSeconds(Timestamp);
            return Dt;
        }
    }
}
