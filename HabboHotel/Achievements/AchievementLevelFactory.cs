using System;
using System.Collections.Generic;
using System.Data;
using ProjectHub.Database.Interfaces;

namespace ProjectHub.HabboHotel.Achievements
{
    public class AchievementLevelFactory
    {
        public static void GetAchievementLevels(out Dictionary<string, Achievement> Achievements)
        {
            Achievements = new Dictionary<string, Achievement>();

            using (IQueryAdapter DbClient = ProjectHub.GetDatabaseManager().GetQueryReactor())
            {
                DbClient.SetQuery("SELECT `id`,`category`,`group_name`,`level`,`reward_pixels`,`reward_points`,`progress_needed`,`game_id` FROM `" + ProjectHub.DbPrefix + "achievements`");
                DataTable DataTable = DbClient.getTable();

                if (DataTable != null)
                {
                    foreach (DataRow DataRow in DataTable.Rows)
                    {
                        int Id = Convert.ToInt32(DataRow["id"]);
                        string Category = Convert.ToString(DataRow["category"]);
                        string GroupName = Convert.ToString(DataRow["group_name"]);
                        int Level = Convert.ToInt32(DataRow["level"]);
                        int RewardPixels = Convert.ToInt32(DataRow["reward_pixels"]);
                        int RewardPoints = Convert.ToInt32(DataRow["reward_points"]);
                        int ProgressNeeded = Convert.ToInt32(DataRow["progress_needed"]);
                        AchievementLevel AchievementLevel = new AchievementLevel(Level, RewardPixels, RewardPoints, ProgressNeeded);

                        if (!Achievements.ContainsKey(GroupName))
                        {
                            Achievement Achievement = new Achievement(Id, GroupName, Category, Convert.ToInt32(DataRow["game_id"]));
                            Achievement.AddLevel(AchievementLevel);
                            Achievements.Add(GroupName, Achievement);
                        }
                        else
                        {
                            Achievements[GroupName].AddLevel(AchievementLevel);
                        }
                    }
                }
            }
        }
    }
}