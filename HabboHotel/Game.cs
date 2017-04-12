using ProjectHub.HabboHotel.Rooms.Chat.WordFilter;

namespace ProjectHub.HabboHotel
{
    public class Game
    {
        private static WordFilterManager WordFilterManager;

        public Game()
        {
            WordFilterManager = new WordFilterManager();
        }

        public WordFilterManager GetWordFilterManager()
        {
            return WordFilterManager;
        }
    }
}
