using ProjectHub.Communication.Packets.Outgoing;
using ProjectHub.Core;
using ProjectHub.Core.Connections;
using ProjectHub.Database.Interfaces;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ProjectHub.HabboHotel.GameClients
{
    public class GameClientManager
    {
        private ConcurrentDictionary<int, GameClient> Clients;
        private ConcurrentDictionary<int, GameClient> UserIdRegister;
        private ConcurrentDictionary<string, GameClient> UsernameRegister;

        private readonly Queue TimedOutConnections;
        private readonly Stopwatch ClientPingStopwatch;

        public GameClientManager()
        {
            Clients = new ConcurrentDictionary<int, GameClient>();
            UserIdRegister = new ConcurrentDictionary<int, GameClient>();
            UsernameRegister = new ConcurrentDictionary<string, GameClient>();
            TimedOutConnections = new Queue();
            ClientPingStopwatch = new Stopwatch();
            ClientPingStopwatch.Start();
        }

        public void OnCycle()
        {
            TestClientConnections();
            HandleTimeouts();
        }

        public GameClient GetClientByUserID(int UserId)
        {
            if (UserIdRegister.ContainsKey(UserId))
            {
                return UserIdRegister[UserId];
            }

            return null;
        }

        public GameClient GetClientByUsername(string Username)
        {
            if (UsernameRegister.ContainsKey(Username.ToLower()))
            {
                return UsernameRegister[Username.ToLower()];
            }

            return null;
        }

        public bool TryGetClient(int ClientId, out GameClient Client)
        {
            return Clients.TryGetValue(ClientId, out Client);
        }

        public bool UpdateClientUsername(GameClient Client, string OldUsername, string NewUsername)
        {
            if (Client == null || !UsernameRegister.ContainsKey(OldUsername.ToLower()))
            {
                return false;
            }

            UsernameRegister.TryRemove(OldUsername.ToLower(), out Client);
            UsernameRegister.TryAdd(NewUsername.ToLower(), Client);

            return true;
        }

        public string GetNameById(int Id)
        {
            GameClient Client = GetClientByUserID(Id);

            if (Client != null)
                return Client.GetHabbo().Username;

            string Username;

            using (IQueryAdapter DbClient = ProjectHub.GetDatabaseManager().GetQueryReactor())
            {
                DbClient.SetQuery("SELECT username FROM " + ProjectHub.DbPrefix + "users WHERE id = @id LIMIT 1");
                DbClient.AddParameter("id", Id);
                Username = DbClient.getString();
            }

            return Username;
        }

        public IEnumerable<GameClient> GetClientsById(Dictionary<int, MessengerBuddy>.KeyCollection Users)
        {
            foreach (int Id in Users)
            {
                GameClient Client = GetClientByUserID(Id);

                if (Client != null)
                {
                    yield return Client;
                }
            }
        }

        public void StaffAlert(ServerPacket Message, int Exclude = 0)
        {
            foreach (GameClient Client in GetClients.ToList())
            {
                if (Client == null || Client.GetHabbo() == null)
                    continue;

                if (Client.GetHabbo().Rank < 2 || Client.GetHabbo().Id == Exclude)
                    continue;

                Client.SendMessage(Message);
            }
        }

        public void ModAlert(string Message)
        {
            foreach (GameClient Client in GetClients.ToList())
            {
                if (Client == null || Client.GetHabbo() == null)
                    continue;

                if (Client.GetHabbo().GetPermissions().HasRight("mod_tool") && !Client.GetHabbo().GetPermissions().HasRight("staff_ignore_mod_alert"))
                {
                    try { Client.SendWhisper(Message, 5); }
                    catch { }
                }
            }
        }

        public void DoAdvertisingReport(GameClient Reporter, GameClient Target)
        {
            if (Reporter == null || Target == null || Reporter.GetHabbo() == null || Target.GetHabbo() == null)
                return;

            StringBuilder Builder = new StringBuilder();
            Builder.Append("New report submitted!\r\r");
            Builder.Append("Reporter: " + Reporter.GetHabbo().Username + "\r");
            Builder.Append("Reported User: " + Target.GetHabbo().Username + "\r\r");
            Builder.Append(Target.GetHabbo().Username + "s last 10 messages:\r\r");

            DataTable GetLogs = null;
            using (IQueryAdapter DbClient = ProjectHub.GetDatabaseManager().GetQueryReactor())
            {
                DbClient.SetQuery("SELECT `message` FROM `" + ProjectHub.DbPrefix + "chatlogs` WHERE `user_id` = '" + Target.GetHabbo().Id + "' ORDER BY `id` DESC LIMIT 10");
                GetLogs = DbClient.getTable();

                if (GetLogs != null)
                {
                    int Number = 11;
                    foreach (DataRow Log in GetLogs.Rows)
                    {
                        Number -= 1;
                        Builder.Append(Number + ": " + Convert.ToString(Log["message"]) + "\r");
                    }
                }
            }

            foreach (GameClient Client in GetClients.ToList())
            {
                if (Client == null || Client.GetHabbo() == null)
                    continue;

                if (Client.GetHabbo().GetPermissions().HasRight("mod_tool") && !Client.GetHabbo().GetPermissions().HasRight("staff_ignore_advertisement_reports"))
                    Client.SendMessage(new MOTDNotificationComposer(Builder.ToString()));
            }
        }


        public void SendMessage(ServerPacket Packet, string Fuse = "")
        {
            foreach (GameClient Client in Clients.Values.ToList())
            {
                if (Client == null || Client.GetHabbo() == null)
                    continue;

                if (!string.IsNullOrEmpty(Fuse))
                {
                    if (!Client.GetHabbo().GetPermissions().HasRight(Fuse))
                        continue;
                }

                Client.SendMessage(Packet);
            }
        }

        public void CreateAndStartClient(int ClientID, ConnectionInformation Connection)
        {
            GameClient Client = new GameClient(ClientID, Connection);
            if (Clients.TryAdd(Client.ConnectionId, Client))
                Client.StartConnection();
            else
                Connection.Dispose();
        }

        public void DisposeConnection(int ClientID)
        {
            GameClient Client = null;

            if (!TryGetClient(ClientID, out Client))
                return;

            if (Client != null)
                Client.Dispose();

            Clients.TryRemove(ClientID, out Client);
        }

        public void LogClonesOut(int UserId)
        {
            GameClient Client = GetClientByUserID(UserId);

            if (Client != null)
                Client.Disconnect();
        }

        public void RegisterClient(GameClient Client, int UserId, string Username)
        {
            if (UsernameRegister.ContainsKey(Username.ToLower()))
                UsernameRegister[Username.ToLower()] = Client;
            else
                UsernameRegister.TryAdd(Username.ToLower(), Client);

            if (UserIdRegister.ContainsKey(UserId))
                UserIdRegister[UserId] = Client;
            else
                UserIdRegister.TryAdd(UserId, Client);
        }

        public void UnregisterClient(int UserId, string Username)
        {
            GameClient Client = null;
            UserIdRegister.TryRemove(UserId, out Client);
            UsernameRegister.TryRemove(Username.ToLower(), out Client);
        }

        public void CloseAll()
        {
            foreach (GameClient Client in GetClients.ToList())
            {
                if (Client == null)
                    continue;

                if (Client.GetHabbo() != null)
                {
                    try
                    {
                        using (IQueryAdapter DbClient = ProjectHub.GetDatabaseManager().GetQueryReactor())
                        {
                            DbClient.RunQuery(Client.GetHabbo().GetQueryString);
                        }

                        Console.Clear();
                    }
                    catch
                    {
                    }
                }
            }

            try
            {
                foreach (GameClient Client in GetClients.ToList())
                {
                    if (Client == null || Client.GetConnection() == null)
                        continue;

                    try
                    {
                        Client.GetConnection().Dispose();
                    }
                    catch { }

                    Console.Clear();

                }
            }
            catch (Exception e)
            {
                Logging.LogError(e.ToString());
            }

            if (Clients.Count > 0)
                Clients.Clear();
        }

        private void TestClientConnections()
        {
            if (ClientPingStopwatch.ElapsedMilliseconds >= 30000)
            {
                ClientPingStopwatch.Restart();

                try
                {
                    List<GameClient> ToPing = new List<GameClient>();

                    foreach (GameClient Client in Clients.Values.ToList())
                    {
                        if (Client.PingCount < 6)
                        {
                            Client.PingCount++;

                            ToPing.Add(Client);
                        }
                        else
                        {
                            lock (TimedOutConnections.SyncRoot)
                            {
                                TimedOutConnections.Enqueue(Client);
                            }
                        }
                    }

                    DateTime start = DateTime.Now;

                    foreach (GameClient Client in ToPing.ToList())
                    {
                        try
                        {
                            Client.SendMessage(new PongComposer());
                        }
                        catch
                        {
                            lock (TimedOutConnections.SyncRoot)
                            {
                                TimedOutConnections.Enqueue(Client);
                            }
                        }
                    }

                }
                catch (Exception e)
                {

                }
            }
        }

        private void HandleTimeouts()
        {
            if (TimedOutConnections.Count > 0)
            {
                lock (TimedOutConnections.SyncRoot)
                {
                    while (TimedOutConnections.Count > 0)
                    {
                        GameClient Client = null;

                        if (TimedOutConnections.Count > 0)
                            Client = (GameClient)TimedOutConnections.Dequeue();

                        if (Client != null)
                            Client.Disconnect();
                    }
                }
            }
        }

        public int Count
        {
            get { return Clients.Count; }
        }

        public ICollection<GameClient> GetClients
        {
            get
            {
                return Clients.Values;
            }
        }
    }
}