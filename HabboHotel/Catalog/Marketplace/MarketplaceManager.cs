using System;
using System.Collections.Generic;

using ProjectHub.Database.Interfaces;

namespace ProjectHub.HabboHotel.Catalog.Marketplace
{
    public class MarketplaceManager
    {
        public List<int> MarketItemKeys = new List<int>();
        public List<MarketOffer> MarketItems = new List<MarketOffer>();
        public Dictionary<int, int> MarketCounts = new Dictionary<int, int>();
        public Dictionary<int, int> MarketAverages = new Dictionary<int, int>();

        public MarketplaceManager()
        {
        }

        public int AvgPriceForSprite(int SpriteID)
        {
            int Num = 0;
            int Num2 = 0;

            if (MarketAverages.ContainsKey(SpriteID) && MarketCounts.ContainsKey(SpriteID))
            {
                if (MarketCounts[SpriteID] > 0)
                {
                    return (MarketAverages[SpriteID] / MarketCounts[SpriteID]);
                }

                return 0;
            }

            using (IQueryAdapter DbClient = ProjectHub.GetDatabaseManager().GetQueryReactor())
            {
                DbClient.SetQuery("SELECT `avgprice` FROM `" + ProjectHub.DbPrefix + "catalog_marketplace_data` WHERE `sprite` = '" + SpriteID + "' LIMIT 1");
                Num = DbClient.getInteger();

                DbClient.SetQuery("SELECT `sold` FROM `" + ProjectHub.DbPrefix + "catalog_marketplace_data` WHERE `sprite` = '" + SpriteID + "' LIMIT 1");
                Num2 = DbClient.getInteger();
            }

            MarketAverages.Add(SpriteID, Num);
            MarketCounts.Add(SpriteID, Num2);

            if (Num2 > 0)
            {
                return Convert.ToInt32(Math.Ceiling((double)(Num / Num2)));
            }
            
            return 0;
        }

        public string FormatTimestampString()
        {
            return FormatTimestamp().ToString().Split(new char[] { ',' })[0];
        }

        public double FormatTimestamp()
        {
            return (ProjectHub.GetUnixTimestamp() - 172800.0);
        }

        public int OfferCountForSprite(int SpriteID)
        {
            Dictionary<int, MarketOffer> Dictionary = new Dictionary<int, MarketOffer>();
            Dictionary<int, int> Dictionary2 = new Dictionary<int, int>();

            foreach (MarketOffer item in this.MarketItems)
            {
                if (Dictionary.ContainsKey(item.SpriteId))
                {
                    if (Dictionary[item.SpriteId].TotalPrice > item.TotalPrice)
                    {
                        Dictionary.Remove(item.SpriteId);
                        Dictionary.Add(item.SpriteId, item);
                    }

                    int num = Dictionary2[item.SpriteId];
                    Dictionary2.Remove(item.SpriteId);
                    Dictionary2.Add(item.SpriteId, num + 1);
                }
                else
                {
                    Dictionary.Add(item.SpriteId, item);
                    Dictionary2.Add(item.SpriteId, 1);
                }
            }
            if (Dictionary2.ContainsKey(SpriteID))
            {
                return Dictionary2[SpriteID];
            }

            return 0;
        }

        public int CalculateComissionPrice(float SellingPrice)
        {
            return Convert.ToInt32(Math.Ceiling(SellingPrice / 100 * 1));
        }
    }
}