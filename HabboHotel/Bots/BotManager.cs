using System;
using System.Linq;
using System.Data;
using System.Collections.Generic;

using ProjectHub.Database.Interfaces;
using ProjectHub.HabboHotel.Rooms.AI.Responses;
using ProjectHub.HabboHotel.Rooms.AI;

namespace ProjectHub.HabboHotel.Bots
{
    public class BotManager
    {
        private List<BotResponse> Responses;

        public BotManager()
        {
            Responses = new List<BotResponse>();
            Init();
        }

        public void Init()
        {
            if (Responses.Count > 0)
            {
                Responses.Clear();
            }

            using (IQueryAdapter DbClient = ProjectHub.GetDatabaseManager().GetQueryReactor())
            {
                DbClient.SetQuery("SELECT `bot_ai`,`chat_keywords`,`response_text`,`response_mode`,`response_beverage` FROM `" + ProjectHub.DbPrefix + "bots_responses`");
                DataTable BotResponses = DbClient.getTable();

                if (BotResponses != null)
                {
                    foreach (DataRow Response in BotResponses.Rows)
                    {
                        Responses.Add(new BotResponse(Convert.ToString(Response["bot_ai"]), Convert.ToString(Response["chat_keywords"]), Convert.ToString(Response["response_text"]), Response["response_mode"].ToString(), Convert.ToString(Response["response_beverage"])));
                    }
                }
            }
        }

        public BotResponse GetResponse(BotAIType AiType, string Message)
        {
            foreach (BotResponse Response in Responses.Where(X => X.AiType == AiType).ToList())
            {
                if (Response.KeywordMatched(Message))
                {
                    return Response;
                }
            }

            return null;
        }
    }
}
