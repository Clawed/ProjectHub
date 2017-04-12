using MySql.Data.MySqlClient;
using ProjectHub.Communication.Encryption;
using ProjectHub.Communication.Encryption.Keys;
using ProjectHub.Core;
using ProjectHub.Core.Connections;
using ProjectHub.Database;
using ProjectHub.HabboHotel;
using ProjectHub.Net.Mus;
using System;
using System.Globalization;
using System.Text;
using System.Threading;

namespace ProjectHub
{
    internal sealed class ProjectHub
    {
        public const string EmuName = "ProjectHub Emulator";
        public const string EmuVersion = "0.0.5";
        public const int EmuBuild = 137;
        public static string DbPrefix = "server_";
        public static string SWFRevision = "";

        public static DateTime ServerStarted;
        public static CultureInfo CultureInfo;
        private static Encoding DefaultEncoding;

        public static ConfigurationData ConfigurationData;
        private static DatabaseManager DatabaseManager;
        public static SettingsData SettingsData;
        public static TextsData TextsData;
        public static MusSocket MusSocket;

        private static ServerStatusUpdater ServerStatusUpdater;
        public static Game Game;
        public static GameCycle GameCycle;
        private static ConnectionManager ConnectionManager;

        public static string PrettyVersion
        {
            get
            {
                return EmuName + ", Version: " + EmuVersion + ", Build: " + EmuBuild;
            }
        }

        public void Initialize()
        {
            ServerStarted = DateTime.Now;
            CultureInfo = CultureInfo.CreateSpecificCulture("en-GB");
            DefaultEncoding = Encoding.Default;

            Console.Title = "Loading " + PrettyVersion;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            Console.WriteLine("                     _   _       _     _____");
            Console.WriteLine("                    | | | |     | |   |  ___|");
            Console.WriteLine("                    | |_| |_   _| |___| |_   _________ _   _");
            Console.WriteLine("                    |  _  | | | |  _  |  _| |  _   _  | | | |");
            Console.WriteLine("                    | | | | |_| | |_| | |___| | | | | | |_| |");
            Console.WriteLine("                    |_| |_|_____|_____|_____|_| |_| |_|_____|");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("                                       " + PrettyVersion);
            Console.WriteLine();

            try
            {
                LoadConfig();
                LoadDatabase();
                LoadSettings();
                LoadTexts();

                Game = new Game();
                HabboEncryptionV2.Initialize(new RSAKeys());

                LoadConnections();
                LoadMus();
                LoadServerStatusUpdater();

                GameCycle = new GameCycle();
                GameCycle.StartLoop();

                TimeSpan TimeUsed = DateTime.Now - ServerStarted;
                Logging.WriteLine(EmuName + " loaded (" + TimeUsed.Seconds + " s, " + TimeUsed.Milliseconds + " ms)");
                Console.Beep();
            }
            catch (InvalidOperationException Error)
            {
                Logging.LogError(Error.ToString());
                Logging.WriteLine("Failed to initialize ProjectHub: " + Error.Message, ConsoleColor.Red);
                Logging.WriteLine("Press any key to shut down ...", ConsoleColor.Red);
                Console.ReadKey(true);
                Environment.Exit(1);
                return;
            }
            catch (Exception Error)
            {
                Logging.LogError(Error.ToString());
                Logging.WriteLine("Fatal error during startup: " + Error, ConsoleColor.Red);
                Logging.WriteLine("Press a key to exit", ConsoleColor.Red);
                Console.ReadKey();
                Environment.Exit(1);
            }
        }

        public static bool EnumToBool(string Enum)
        {
            return (Enum == "1");
        }

        public static string BoolToEnum(bool Bool)
        {
            return (Bool == true ? "1" : "0");
        }

        public static void LoadConfig()
        {
            Logging.Write("Loading config.ini file");

            try
            {
                ConfigurationData = new ConfigurationData(@"config.ini");
                Logging.WriteSimpleLine(" - Completed!");
            }
            catch (Exception Error)
            {
                Logging.WriteSimpleLine(" - Incomplete!");
                Logging.LogError(Error.ToString());
                Logging.WriteLine("Please check your configuration file - some values appear to be missing.", ConsoleColor.Red);
                Logging.WriteLine("Press any key to shut down ...", ConsoleColor.Red);
                Console.ReadKey(true);
                Environment.Exit(1);
                return;
            }
        }

        public static void LoadDatabase()
        {
            Logging.Write("Connecting to database");

            try
            {
                var connectionString = new MySqlConnectionStringBuilder
                {
                    ConnectionTimeout = 10,
                    Database = GetConfig().Data["db.name"],
                    DefaultCommandTimeout = 30,
                    Logging = false,
                    MaximumPoolSize = uint.Parse(GetConfig().Data["db.pool.maxsize"]),
                    MinimumPoolSize = uint.Parse(GetConfig().Data["db.pool.minsize"]),
                    Password = GetConfig().Data["db.password"],
                    Pooling = true,
                    Port = uint.Parse(GetConfig().Data["db.port"]),
                    Server = GetConfig().Data["db.hostname"],
                    UserID = GetConfig().Data["db.username"],
                    AllowZeroDateTime = true,
                    ConvertZeroDateTime = true,
                };

                DatabaseManager = new DatabaseManager(connectionString.ToString());

                Logging.WriteSimpleLine(" - Completed!");
            }
            catch (Exception Error)
            {
                Logging.WriteSimpleLine(" - Incomplete!");
                Logging.LogError(Error.ToString());
                Logging.WriteLine("Failed to connect to MySQL server!", ConsoleColor.Red);
                Logging.WriteLine("Press any key to shut down ...", ConsoleColor.Red);
                Console.ReadKey(true);
                Environment.Exit(1);
                return;
            }
        }

        public static void LoadSettings()
        {
            Logging.Write("Loading server settings");

            try
            {
                SettingsData = new SettingsData();
                Logging.WriteSimpleLine(" - Completed!");
            }
            catch (Exception Error)
            {
                Logging.WriteSimpleLine(" - Incomplete!");
                Logging.LogError(Error.ToString());
                Logging.WriteLine("Could not load server settings!", ConsoleColor.Red);
                Logging.WriteLine("Press any key to shut down ...", ConsoleColor.Red);
                Console.ReadKey(true);
                Environment.Exit(1);
                return;
            }
        }

        public static void LoadTexts()
        {
            Logging.Write("Loading server texts");

            try
            {
                TextsData = new TextsData();
                Logging.WriteSimpleLine(" - Completed!");
            }
            catch (Exception Error)
            {
                Logging.WriteSimpleLine(" - Incomplete!");
                Logging.LogError(Error.ToString());
                Logging.WriteLine("Could not load server settings!", ConsoleColor.Red);
                Logging.WriteLine("Press any key to shut down ...", ConsoleColor.Red);
                Console.ReadKey(true);
                Environment.Exit(1);
                return;
            }
        }

        public static void LoadConnections()
        {
            Logging.Write("Listening for connections on (" + GetSettingsData().Data["game.ip"] + ":" + GetSettingsData().Data["game.port"] + ")");

            try
            {
                ConnectionManager = new ConnectionManager(int.Parse(GetSettingsData().Data["game.port"]), int.Parse(GetSettingsData().Data["game.con.limit"]), int.Parse(GetSettingsData().Data["game.con.perip"]), true);
                ConnectionManager.Init();
                Logging.WriteSimpleLine(" - Completed!");
            }
            catch (Exception Error)
            {
                Logging.WriteSimpleLine(" - Incomplete!");
                Logging.LogError(Error.ToString());
                Logging.WriteLine("Could not load connection socket!", ConsoleColor.Red);
                Logging.WriteLine("Press any key to shut down ...", ConsoleColor.Red);
                Console.ReadKey(true);
                Environment.Exit(1);
                return;
            }
        }

        public static void LoadMus()
        {
            Logging.Write("Listening for MUS connections on (" + GetSettingsData().Data["mus.ip"] + ":" + GetSettingsData().Data["mus.port"] + ")");

            try
            {
                MusSocket = new MusSocket(GetSettingsData().Data["mus.ip"], int.Parse(GetSettingsData().Data["mus.port"]), GetSettingsData().Data["mus.allowedips"].Split(Convert.ToChar(";")), 0);
                Logging.WriteSimpleLine(" - Completed!");
            }
            catch (Exception Error)
            {
                Logging.WriteSimpleLine(" - Incomplete!");
                Logging.LogError(Error.ToString());
                Logging.WriteLine("Could not load mus socket!", ConsoleColor.Red);
                Logging.WriteLine("Press any key to shut down ...", ConsoleColor.Red);
                Console.ReadKey(true);
                Environment.Exit(1);
                return;
            }
        }

        public static void LoadServerStatusUpdater()
        {
            Logging.Write("Starting background server updater");

            try
            {
                ServerStatusUpdater = new ServerStatusUpdater();
                Logging.WriteSimpleLine(" - Completed!");
            }
            catch (Exception Error)
            {
                Logging.WriteSimpleLine(" - Incomplete!");
                Logging.LogError(Error.ToString());
                Logging.WriteLine("Could not start background server updater!", ConsoleColor.Red);
                Logging.WriteLine("Press any key to shut down ...", ConsoleColor.Red);
                Console.ReadKey(true);
                Environment.Exit(1);
                return;
            }
        }

        public static ConfigurationData GetConfig()
        {
            return ConfigurationData;
        }

        public static SettingsData GetSettingsData()
        {
            return SettingsData;
        }

        public static DatabaseManager GetDatabaseManager()
        {
            return DatabaseManager;
        }

        public static Encoding GetDefaultEncoding()
        {
            return DefaultEncoding;
        }

        public static ConnectionManager GetConnectionManager()
        {
            return ConnectionManager;
        }

        public static Game GetGame()
        {
            return Game;
        }

        public static double GetUnixTimestamp()
        {
            return (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
        }

        public static void Shutdown()
        {
            Console.Clear();
            Console.Title = "Closing down " + PrettyVersion;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            Console.WriteLine("                     _   _       _     _____");
            Console.WriteLine("                    | | | |     | |   |  ___|");
            Console.WriteLine("                    | |_| |_   _| |___| |_   _________ _   _");
            Console.WriteLine("                    |  _  | | | |  _  |  _| |  _   _  | | | |");
            Console.WriteLine("                    | | | | |_| | |_| | |___| | | | | | |_| |");
            Console.WriteLine("                    |_| |_|_____|_____|_____|_| |_| |_|_____|");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("                                       " + PrettyVersion);
            Console.WriteLine();

            Logging.WriteLine("Perfroming secure shutdown.");
            Logging.WriteLine("Saving all cached data.");

            Logging.Write("Stoping background server updater");

            try
            {
                ServerStatusUpdater.Dispose();
                Logging.WriteSimpleLine(" - Completed!");
            }
            catch (Exception Error)
            {
                Logging.WriteSimpleLine(" - Incomplete!");
                Logging.LogError(Error.ToString());
                Logging.WriteLine("Could not stop background server updater!", ConsoleColor.Red);
                Logging.WriteLine("Press any key to shut down ...", ConsoleColor.Red);
                Console.ReadKey(true);
                Environment.Exit(1);
                return;
            }

            Thread.Sleep(5000);
            Logging.WriteLine(EmuName + " has successfully shutdown.");
            Thread.Sleep(1000);
            Environment.Exit(0);
        }
    }
}
