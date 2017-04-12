using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics;

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
    }
}