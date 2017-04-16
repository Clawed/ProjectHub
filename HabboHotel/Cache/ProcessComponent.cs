using System;
using System.Linq;
using System.Threading;
using System.Collections.Generic;

using ProjectHub.HabboHotel.Users;
using ProjectHub.Core;

namespace ProjectHub.HabboHotel.Cache
{
    sealed class ProcessComponent
    {
        private Timer Timer = null;
        private bool TimerRunning = false;
        private bool TimerLagging = false;
        private bool Disabled = false;
        private AutoResetEvent ResetEvent = new AutoResetEvent(true);
        private static int RuntimeInSec = 1200;

        public ProcessComponent()
        {
            Timer = new Timer(new TimerCallback(Run), null, RuntimeInSec * 1000, RuntimeInSec * 1000);
        }

        public void Run(object State)
        {
            try
            {
                if (Disabled)
                {
                    return;
                }

                if (TimerRunning)
                {
                    TimerLagging = true;
                    return;
                }

                ResetEvent.Reset();
                List<UserCache> CacheList = ProjectHub.GetGame().GetCacheManager().GetUserCache().ToList();

                if (CacheList.Count > 0)
                {
                    foreach (UserCache Cache in CacheList)
                    {
                        try
                        {
                            if (Cache == null)
                            {
                                continue;
                            }

                            UserCache Temp = null;

                            if (Cache.IsExpired())
                            {
                                ProjectHub.GetGame().GetCacheManager().TryRemoveUser(Cache.Id, out Temp);
                            }

                            Temp = null;
                        }
                        catch (Exception Error)
                        {
                            Logging.LogError(Error.ToString());
                        }
                    }
                }

                CacheList = null;
                List<Habbo> CachedUsers = ProjectHub.GetUsersCached().ToList();

                if (CachedUsers.Count > 0)
                {
                    foreach (Habbo Data in CachedUsers)
                    {
                        try
                        {
                            if (Data == null)
                            {
                                continue;
                            }

                            Habbo Temp = null;

                            if (Data.CacheExpired())
                            {
                                ProjectHub.RemoveFromCache(Data.Id, out Temp);
                            }

                            if (Temp != null)
                            {
                                Temp.Dispose();
                            }

                            Temp = null;
                        }
                        catch (Exception Error)
                        {
                            Logging.LogError(Error.ToString());
                        }
                    }
                }

                CachedUsers = null;
                TimerRunning = false;
                TimerLagging = false;
                ResetEvent.Set();
            }
            catch (Exception Error)
            {
                Logging.LogError(Error.ToString());
            }
        }

        public void Dispose()
        {
            try
            {
                ResetEvent.WaitOne(TimeSpan.FromMinutes(5));
            }
            catch { }

            Disabled = true;

            try
            {
                if (Timer != null)
                {
                    Timer.Dispose();
                }
            }
            catch { }

            Timer = null;
        }
    }
}