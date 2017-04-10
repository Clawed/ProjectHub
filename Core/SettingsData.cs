using System.Collections.Generic;
using ProjectHub.Database.Interfaces;
using System.Data;

namespace ProjectHub.Core
{
    public class SettingsData
    {
        public Dictionary<string, string> Data;

        public SettingsData()
        {
            Data = new Dictionary<string, string>();
            Data.Clear();

            using (IQueryAdapter dbClient = ProjectHub.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `" + ProjectHub.DbPrefix + "settings`");
                DataTable ConfigData = dbClient.getTable();

                if (ConfigData != null)
                {
                    foreach (DataRow Data2 in ConfigData.Rows)
                    {
                        Data.Add(Data2[1].ToString(), Data2[2].ToString());
                    }
                }
            }
            return;
        }
    }
}