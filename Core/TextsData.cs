using System.Collections.Generic;
using ProjectHub.Database.Interfaces;
using System.Data;

namespace ProjectHub.Core
{
    public class TextsData
    {
        public Dictionary<string, string> Data;

        public TextsData()
        {
            Data = new Dictionary<string, string>();
            Data.Clear();

            using (IQueryAdapter dbClient = ProjectHub.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `" + ProjectHub.DbPrefix + "texts`");
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