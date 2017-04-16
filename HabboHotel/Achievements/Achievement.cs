using System.Collections.Generic;

namespace ProjectHub.HabboHotel.Achievements
{
    public class Achievement
    {
        public int Id;
        public string Category;
        public string GroupName;
        public int GameId;
        public Dictionary<int, AchievementLevel> Levels;

        public Achievement(int _Id, string _GroupName, string _Category, int _GameId)
        {
            Id = _Id;
            GroupName = _GroupName;
            Category = _Category;
            GameId = _GameId;
            Levels = new Dictionary<int, AchievementLevel>();
        }

        public void AddLevel(AchievementLevel Level)
        {
            Levels.Add(Level.Level, Level);
        }
    }
}