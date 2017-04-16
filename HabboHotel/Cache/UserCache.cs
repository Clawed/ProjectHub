using System;

namespace ProjectHub.HabboHotel.Cache
{
    public class UserCache
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Motto { get; set; }
        public string Look { get; set; }
        public DateTime AddedTime { get; set; }

        public UserCache(int _Id, string _Username, string _Motto, string _Look)
        {
            Id = _Id;
            Username = _Username;
            Motto = _Motto;
            Look = _Look;
            AddedTime = DateTime.Now;
        }

        public bool IsExpired()
        {
            TimeSpan CacheTime = DateTime.Now - AddedTime;

            return CacheTime.TotalMinutes >= 30;
        }
    }
}
