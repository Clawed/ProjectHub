namespace ProjectHub.HabboHotel.Achievements
{
    public struct AchievementLevel
    {
        public readonly int Level;
        public readonly int Requirement;
        public readonly int RewardPixels;
        public readonly int RewardPoints;

        public AchievementLevel(int _Level, int _RewardPixels, int _RewardPoints, int _Requirement)
        {
            Level = _Level;
            RewardPixels = _RewardPixels;
            RewardPoints = _RewardPoints;
            Requirement = _Requirement;
        }
    }
}