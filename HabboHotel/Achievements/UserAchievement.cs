namespace ProjectHub.HabboHotel.Achievements
{
    public class UserAchievement
    {
        public readonly string AchievementGroup;
        public int Level;
        public int Progress;

        public UserAchievement(string _AchievementGroup, int _Level, int _Progress)
        {
            AchievementGroup = _AchievementGroup;
            Level = _Level;
            Progress = _Progress;
        }
    }
}