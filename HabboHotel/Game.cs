using ProjectHub.Core;
using ProjectHub.HabboHotel.Rooms.Chat.WordFilter;
using System;

namespace ProjectHub.HabboHotel
{
    public class Game
    {
        private static WordFilterManager WordFilterManager;

        public Game()
        {
            LoadWordFilter();
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

        public WordFilterManager GetWordFilterManager()
        {
            return WordFilterManager;
        }
    }
}
