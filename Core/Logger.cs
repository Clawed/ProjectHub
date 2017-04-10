using ProjectHub.Database.Interfaces;

namespace ProjectHub.Core
{
    public class Logger
    {
        public Logger(string Type, string Command, string Extra, int UserId = 0)
        {
            using (IQueryAdapter DbClient = ProjectHub.GetDatabaseManager().GetQueryReactor())
            {
                DbClient.SetQuery("INSERT INTO " + ProjectHub.DbPrefix + "logs VALUES (NULL, @type, @command, @extra, @user_id, UNIX_TIMESTAMP());");
                DbClient.AddParameter("type", Type);
                DbClient.AddParameter("command", Command);
                DbClient.AddParameter("extra", Extra);
                DbClient.AddParameter("user_id", UserId);
                DbClient.RunQuery();
            }
        }
    }
}
