using ProjectHub.Database.Interfaces;
using ProjectHub.HabboHotel.GameClients;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectHub.HabboHotel.Achievements
{
    public class AchievementManager
    {
        public Dictionary<string, Achievement> Achievements;

        public AchievementManager()
        {
            Achievements = new Dictionary<string, Achievement>();
            LoadAchievements();
        }

        public void LoadAchievements()
        {
            AchievementLevelFactory.GetAchievementLevels(out Achievements);
        }

        public bool ProgressAchievement(GameClient Session, string AchievementGroup, int ProgressAmount, bool FromZero = false)
        {
            if (!Achievements.ContainsKey(AchievementGroup) || Session == null)
            {
                return false;
            }

            Achievement AchievementData = null;
            AchievementData = Achievements[AchievementGroup];
            UserAchievement UserData = Session.GetHabbo().GetAchievementData(AchievementGroup);

            if (UserData == null)
            {
                UserData = new UserAchievement(AchievementGroup, 0, 0);
                Session.GetHabbo().Achievements.TryAdd(AchievementGroup, UserData);
            }

            int TotalLevels = AchievementData.Levels.Count;

            if (UserData != null && UserData.Level == TotalLevels)
            {
                return false;
            }

            int TargetLevel = (UserData != null ? UserData.Level + 1 : 1);

            if (TargetLevel > TotalLevels)
            {
                TargetLevel = TotalLevels;
            }

            AchievementLevel TargetLevelData = AchievementData.Levels[TargetLevel];
            int NewProgress = 0;

            if (FromZero)
            {
                NewProgress = ProgressAmount;
            }
            else
            {
                NewProgress = (UserData != null ? UserData.Progress + ProgressAmount : ProgressAmount);
            }

            int NewLevel = (UserData != null ? UserData.Level : 0);
            int NewTarget = NewLevel + 1;

            if (NewTarget > TotalLevels)
            {
                NewTarget = TotalLevels;
            }

            if (NewProgress >= TargetLevelData.Requirement)
            {
                NewLevel++;
                NewTarget++;
                int ProgressRemainder = NewProgress - TargetLevelData.Requirement;
                NewProgress = 0;

                if (TargetLevel == 1)
                {
                    Session.GetHabbo().GetBadgeComponent().GiveBadge(AchievementGroup + TargetLevel, true, Session);
                }
                else
                {
                    Session.GetHabbo().GetBadgeComponent().RemoveBadge(Convert.ToString(AchievementGroup + (TargetLevel - 1)));
                    Session.GetHabbo().GetBadgeComponent().GiveBadge(AchievementGroup + TargetLevel, true, Session);
                }

                if (NewTarget > TotalLevels)
                {
                    NewTarget = TotalLevels;
                }

                Session.SendMessage(new AchievementUnlockedComposer(AchievementData, TargetLevel, TargetLevelData.RewardPoints, TargetLevelData.RewardPixels));
                Session.GetHabbo().GetMessenger().BroadcastAchievement(Session.GetHabbo().Id, Users.Messenger.MessengerEventTypes.ACHIEVEMENT_UNLOCKED, AchievementGroup + TargetLevel);

                using (IQueryAdapter DbClient = ProjectHub.GetDatabaseManager().GetQueryReactor())
                {
                    DbClient.SetQuery("REPLACE INTO `" + ProjectHub.DbPrefix + "user_achievements` VALUES ('" + Session.GetHabbo().Id + "', @group, '" + NewLevel + "', '" + NewProgress + "')");
                    DbClient.AddParameter("group", AchievementGroup);
                    DbClient.RunQuery();
                }

                UserData.Level = NewLevel;
                UserData.Progress = NewProgress;
                Session.GetHabbo().Duckets += TargetLevelData.RewardPixels;
                Session.GetHabbo().GetStats().AchievementPoints += TargetLevelData.RewardPoints;
                Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Duckets, TargetLevelData.RewardPixels));
                Session.SendMessage(new AchievementScoreComposer(Session.GetHabbo().GetStats().AchievementPoints));
                AchievementLevel NewLevelData = AchievementData.Levels[NewTarget];
                Session.SendMessage(new AchievementProgressedComposer(AchievementData, NewTarget, NewLevelData, TotalLevels, Session.GetHabbo().GetAchievementData(AchievementGroup)));

                return true;
            }
            else
            {
                UserData.Level = NewLevel;
                UserData.Progress = NewProgress;

                using (IQueryAdapter DbClient = ProjectHub.GetDatabaseManager().GetQueryReactor())
                {
                    DbClient.SetQuery("REPLACE INTO `" + ProjectHub.DbPrefix + "user_achievements` VALUES ('" + Session.GetHabbo().Id + "', @group, '" + NewLevel + "', '" + NewProgress + "')");
                    DbClient.AddParameter("group", AchievementGroup);
                    DbClient.RunQuery();
                }

                Session.SendMessage(new AchievementProgressedComposer(AchievementData, TargetLevel, TargetLevelData, TotalLevels, Session.GetHabbo().GetAchievementData(AchievementGroup)));
            }

            return false;
        }

        public ICollection<Achievement> GetGameAchievements(int GameId)
        {
            List<Achievement> GameAchievements = new List<Achievement>();

            foreach (Achievement Achievement in Achievements.Values.ToList())
            {
                if (Achievement.Category == "games" && Achievement.GameId == GameId)
                {
                    GameAchievements.Add(Achievement);
                }
            }

            return GameAchievements;
        }
    }
}
