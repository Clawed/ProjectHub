using System;
using System.Data;
using ProjectHub.Database.Interfaces;
using ProjectHub.HabboHotel.Catalog;
using ProjectHub.HabboHotel.Users.Inventory.Bots;
using ProjectHub.HabboHotel.Rooms.AI;



namespace ProjectHub.HabboHotel.Items.Utilities
{
    public static class BotUtility
    {
        public static Bot CreateBot(ItemData Data, int OwnerId)
        {
            DataRow BotData = null;
            CatalogBot CataBot = null;

            if (!ProjectHub.GetGame().GetCatalog().TryGetBot(Data.Id, out CataBot))
            {
                return null;
            }

            using (IQueryAdapter DbClient = ProjectHub.GetDatabaseManager().GetQueryReactor())
            {
                DbClient.SetQuery("INSERT INTO " + ProjectHub.DbPrefix + "bots (`user_id`,`name`,`motto`,`look`,`gender`,`ai_type`) VALUES ('" + OwnerId + "', '" + CataBot.Name + "', '" + CataBot.Motto + "', '" + CataBot.Figure + "', '" + CataBot.Gender + "', '" + CataBot.AIType + "')");
                int Id = Convert.ToInt32(DbClient.InsertQuery());

                DbClient.SetQuery("SELECT `id`,`user_id`,`name`,`motto`,`look`,`gender` FROM `" + ProjectHub.DbPrefix + "bots` WHERE `user_id` = '" + OwnerId + "' AND `id` = '" + Id + "' LIMIT 1");
                BotData = DbClient.getRow();
            }

            return new Bot(Convert.ToInt32(BotData["id"]), Convert.ToInt32(BotData["user_id"]), Convert.ToString(BotData["name"]), Convert.ToString(BotData["motto"]), Convert.ToString(BotData["look"]), Convert.ToString(BotData["gender"]));
        }


        public static BotAIType GetAIFromString(string Type)
        {
            switch (Type)
            {
                case "pet":
                    return BotAIType.PET;
                case "generic":
                    return BotAIType.GENERIC;
                case "bartender":
                    return BotAIType.BARTENDER;
                case "casino_bot":
                    return BotAIType.CASINO_BOT;
                default:
                    return BotAIType.GENERIC;
            }
        }
    }
}