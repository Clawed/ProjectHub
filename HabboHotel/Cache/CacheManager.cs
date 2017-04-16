using ProjectHub.Database.Interfaces;
using ProjectHub.HabboHotel.GameClients;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;

namespace ProjectHub.HabboHotel.Cache
{
    public class CacheManager
    {
        private ConcurrentDictionary<int, UserCache> UsersCached;
        private ProcessComponent Process;

        public CacheManager()
        {
            UsersCached = new ConcurrentDictionary<int, UserCache>();
            Process = new ProcessComponent();
        }

        public bool ContainsUser(int Id)
        {
            return UsersCached.ContainsKey(Id);
        }

        public UserCache GenerateUser(int Id)
        {
            UserCache User = null;

            if (UsersCached.ContainsKey(Id))
            {
                if (TryGetUser(Id, out User))
                {
                    return User;
                }
            }

            GameClient Client = ProjectHub.GetGame().GetClientManager().GetClientByUserID(Id);

            if (Client != null)
            {
                if (Client.GetHabbo() != null)
                {
                    User = new UserCache(Id, Client.GetHabbo().Username, Client.GetHabbo().Motto, Client.GetHabbo().Look);
                    UsersCached.TryAdd(Id, User);

                    return User;
                }
            }

            using (IQueryAdapter DbClient = ProjectHub.GetDatabaseManager().GetQueryReactor())
            {
                DbClient.SetQuery("SELECT `username`, `motto`, `look` FROM " + ProjectHub.DbPrefix + "users WHERE id = @id LIMIT 1");
                DbClient.AddParameter("id", Id);

                DataRow DataRow = DbClient.getRow();

                if (DataRow != null)
                {
                    User = new UserCache(Id, DataRow["username"].ToString(), DataRow["motto"].ToString(), DataRow["look"].ToString());
                    UsersCached.TryAdd(Id, User);
                }

                DataRow = null;
            }

            return User;
        }

        public bool TryRemoveUser(int Id, out UserCache User)
        {
            return UsersCached.TryRemove(Id, out User);
        }

        public bool TryGetUser(int Id, out UserCache User)
        {
            return UsersCached.TryGetValue(Id, out User);
        }

        public ICollection<UserCache> GetUserCache()
        {
            return UsersCached.Values;
        }
    }
}