using System;

namespace ProjectHub.Core
{
    public class ConsoleCommandHandler
    {
        public static void InvokeCommand(string Data)
        {
            try
            {
                string[] Params = Data.Split(' ');

                switch (Params[0].ToLower())
                {
                    case "close":
                    case "stop":
                    case "shutdown":
                        {
                            new Logger("console", Params[0], "NULL");
                            ProjectHub.Shutdown();
                            break;
                        }
                    case "commands":
                        {
                            Logging.WriteLine("Current console command list:", ConsoleColor.DarkMagenta);
                            Logging.WriteLine("- commands - Lists all console commands!", ConsoleColor.DarkMagenta);
                            Logging.WriteLine("- close, stop, shutdown - Shuts down server securely!", ConsoleColor.DarkMagenta);
                            Logging.WriteLine("- update_settings, reload_settings - Reloads server settings from database!", ConsoleColor.DarkMagenta);
                            Logging.WriteLine("- update_texts, reload_texts - Reloads server texts from database!", ConsoleColor.DarkMagenta);
                            Logging.WriteLine("- uptime - Shows current uptime of server!", ConsoleColor.DarkMagenta);
                            new Logger("console", Params[0], "NULL");
                            break;
                        }
                    case "update_settings":
                    case "reload_settings":
                        {
                            Logging.Write("Updating server settings", ConsoleColor.DarkMagenta);
                            ProjectHub.SettingsData = new SettingsData();
                            new Logger("console", Params[0], "NULL");
                            Logging.WriteSimpleLine(" - Completed!", ConsoleColor.DarkMagenta);
                            break;
                        }
                    case "update_texts":
                    case "reload_texts":
                        {
                            Logging.Write("Updating server texts", ConsoleColor.DarkMagenta);
                            ProjectHub.TextsData = new TextsData();
                            new Logger("console", Params[0], "NULL");
                            Logging.WriteSimpleLine(" - Completed!", ConsoleColor.DarkMagenta);
                            break;
                        }
                    case "uptime":
                        {
                            TimeSpan Uptime = DateTime.Now - ProjectHub.ServerStarted;
                            Logging.WriteLine("Uptime: Days: " + Uptime.Days + ", Hours: " + Uptime.Days + ", Minutes: " + Uptime.Minutes + ", Seconds: " + Uptime.Seconds);
                            new Logger("console", Params[0], "NULL");
                            break;
                        }
                    default:
                        {
                            if (string.IsNullOrEmpty(Data))
                            {
                                Logging.WriteLine("Empty command data! Type commands for list of commands!");
                            }
                            else
                            {
                                Logging.WriteLine(Params[0].ToLower() + " is an unknown or unsupported command! Type commands for list of commands!");
                            }

                            break;
                        }
                }
            }
            catch
            {
            }
        }
    }
}
