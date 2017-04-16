using ProjectHub.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace ProjectHub.HabboHotel.Catalog.Clothing
{
    public class ClothingManager
    {
        private Dictionary<int, ClothingItem> Clothing;

        public ClothingManager()
        {
            Clothing = new Dictionary<int, ClothingItem>();

            if (Clothing.Count > 0)
            {
                Clothing.Clear();
            }

            DataTable GetClothing = null;

            using (IQueryAdapter DbClient = ProjectHub.GetDatabaseManager().GetQueryReactor())
            {
                DbClient.SetQuery("SELECT `id`,`clothing_name`,`clothing_parts` FROM `" + ProjectHub.DbPrefix + "catalog_clothing`");
                GetClothing = DbClient.getTable();
            }

            if (GetClothing != null)
            {
                foreach (DataRow DataRow in GetClothing.Rows)
                {
                    Clothing.Add(Convert.ToInt32(DataRow["id"]), new ClothingItem(Convert.ToInt32(DataRow["id"]), Convert.ToString(DataRow["clothing_name"]), Convert.ToString(DataRow["clothing_parts"])));
                }
            }
        }

        public bool TryGetClothing(int ItemId, out ClothingItem Clothing)
        {
            if (this.Clothing.TryGetValue(ItemId, out Clothing))
            {
                return true;
            }

            return false;
        }
    }
}
