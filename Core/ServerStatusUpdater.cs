using ProjectHub.Database.Interfaces;
using System;
using System.Threading;

namespace ProjectHub.Core
{
    internal sealed class ServerStatusUpdater : IDisposable
    {
        private const int UPDATE_IN_SECS = 15;
        private Timer Timer;

        public ServerStatusUpdater()
        {
            Timer = new Timer(new TimerCallback(OnTick), null, TimeSpan.FromSeconds(UPDATE_IN_SECS), TimeSpan.FromSeconds(UPDATE_IN_SECS));
            Console.Title = ProjectHub.PrettyVersion + ", 0 users online, 0 rooms loaded, 0 day(s) 0 hour(s) 0 minute(s) uptime";
        }

        public void OnTick(object Obj)
        {
            UpdateServerStatus();
        }

        private void UpdateServerStatus()
        {
            TimeSpan Uptime = DateTime.Now - ProjectHub.ServerStarted;

            int UsersOnline = 0;
            int RoomCount = 0;

            Console.Title = ProjectHub.PrettyVersion + ", " + UsersOnline + " users online, " + RoomCount + " rooms loaded, " + Uptime.Days + " day(s) " + Uptime.Hours + " hour(s) " + Uptime.Minutes + " minute(s) uptime";

            using (IQueryAdapter DbClient = ProjectHub.GetDatabaseManager().GetQueryReactor())
            {
                DbClient.SetQuery("UPDATE `server_status` SET `users_online` = @users, `rooms_loaded` = @rooms LIMIT 1;");
                DbClient.AddParameter("users", UsersOnline);
                DbClient.AddParameter("rooms", RoomCount);
                DbClient.RunQuery();
            }
        }

        public void Dispose()
        {
            using (IQueryAdapter DbClient = ProjectHub.GetDatabaseManager().GetQueryReactor())
            {
                DbClient.RunQuery("UPDATE `server_status` SET `users_online` = '0', `rooms_loaded` = '0'");
            }

            Timer.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}