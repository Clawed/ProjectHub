using ProjectHub.Core;
using ProjectHub.HabboHotel.Achievements;
using ProjectHub.HabboHotel.Badges;
using ProjectHub.HabboHotel.Bots;
using ProjectHub.HabboHotel.Cache;
using ProjectHub.HabboHotel.Rooms.Chat.WordFilter;
using System;

namespace ProjectHub.HabboHotel
{
    public class Game
    {
        private static WordFilterManager WordFilterManager;
        private static AchievementManager AchievementManager;
        private static BadgeManager BadgeManager;
        private static BotManager BotManager;
        private static CacheManager CacheManager;

        public Game()
        {
            LoadWordFilter();
            LoadAchievements();
            LoadBadges();
            LoadBots();
            LoadCache();
        }

        public static void LoadWordFilter()
        {
            Logging.Write("Loading wordfilter");

            try
            {
                WordFilterManager = new WordFilterManager();
                Logging.WriteSimpleLine(" - Completed!");
            }
            catch (Exception Error)
            {
                Logging.WriteSimpleLine(" - Incomplete!");
                Logging.LogError(Error.ToString());
                Logging.WriteLine("Could not load wordfilter!", ConsoleColor.Red);
                Logging.WriteLine("Press any key to shut down ...", ConsoleColor.Red);
                Console.ReadKey(true);
                Environment.Exit(1);
                return;
            }
        }

        public static void LoadAchievements()
        {
            Logging.Write("Loading achievements");

            try
            {
                AchievementManager = new AchievementManager();
                Logging.WriteSimpleLine(" - Completed!");
            }
            catch (Exception Error)
            {
                Logging.WriteSimpleLine(" - Incomplete!");
                Logging.LogError(Error.ToString());
                Logging.WriteLine("Could not load achievements!", ConsoleColor.Red);
                Logging.WriteLine("Press any key to shut down ...", ConsoleColor.Red);
                Console.ReadKey(true);
                Environment.Exit(1);
                return;
            }
        }

        public static void LoadBadges()
        {
            Logging.Write("Loading badges");

            try
            {
                BadgeManager = new BadgeManager();
                Logging.WriteSimpleLine(" - Completed!");
            }
            catch (Exception Error)
            {
                Logging.WriteSimpleLine(" - Incomplete!");
                Logging.LogError(Error.ToString());
                Logging.WriteLine("Could not load badges!", ConsoleColor.Red);
                Logging.WriteLine("Press any key to shut down ...", ConsoleColor.Red);
                Console.ReadKey(true);
                Environment.Exit(1);
                return;
            }
        }

        public static void LoadBots()
        {
            Logging.Write("Loading bots");

            try
            {
                BotManager = new BotManager();
                Logging.WriteSimpleLine(" - Completed!");
            }
            catch (Exception Error)
            {
                Logging.WriteSimpleLine(" - Incomplete!");
                Logging.LogError(Error.ToString());
                Logging.WriteLine("Could not load bots!", ConsoleColor.Red);
                Logging.WriteLine("Press any key to shut down ...", ConsoleColor.Red);
                Console.ReadKey(true);
                Environment.Exit(1);
                return;
            }
        }

        public static void LoadCache()
        {
            Logging.Write("Loading cache");

            try
            {
                CacheManager = new CacheManager();
                Logging.WriteSimpleLine(" - Completed!");
            }
            catch (Exception Error)
            {
                Logging.WriteSimpleLine(" - Incomplete!");
                Logging.LogError(Error.ToString());
                Logging.WriteLine("Could not load cache!", ConsoleColor.Red);
                Logging.WriteLine("Press any key to shut down ...", ConsoleColor.Red);
                Console.ReadKey(true);
                Environment.Exit(1);
                return;
            }
        }

        public WordFilterManager GetWordFilterManager()
        {
            return WordFilterManager;
        }

        public AchievementManager GetAchievementManager()
        {
            return AchievementManager;
        }

        public BadgeManager GetBadgeManager()
        {
            return BadgeManager;
        }

        public BotManager GetBotManager()
        {
            return BotManager;
        }

        public CacheManager GetCacheManager()
        {
            return CacheManager;
        }
    }
}
