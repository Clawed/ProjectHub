using System;
using System.Data;
using System.Collections.Generic;

using ProjectHub.Database.Interfaces;

namespace ProjectHub.HabboHotel.Badges
{
    public class BadgeManager
    {
        private readonly Dictionary<string, BadgeDefinition> Badges;

        public BadgeManager()
        {
            Badges = new Dictionary<string, BadgeDefinition>();

            using (IQueryAdapter DbClient = ProjectHub.GetDatabaseManager().GetQueryReactor())
            {
                DbClient.SetQuery("SELECT * FROM `" +  ProjectHub.DbPrefix + "badge_definitions`;");
                DataTable GetBadges = DbClient.getTable();

                foreach (DataRow Row in GetBadges.Rows)
                {
                    string BadgeCode = Convert.ToString(Row["code"]).ToUpper();

                    if (!Badges.ContainsKey(BadgeCode))
                    {
                        Badges.Add(BadgeCode, new BadgeDefinition(BadgeCode, Convert.ToString(Row["required_right"])));
                    }
                }
            }
        }

        public bool TryGetBadge(string BadgeCode, out BadgeDefinition Badge)
        {
            return Badges.TryGetValue(BadgeCode.ToUpper(), out Badge);
        }
    }
}