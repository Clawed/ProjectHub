using System;
using ProjectHub.Core;

namespace ProjectHub
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                ProjectHub ProjectHub = new ProjectHub();
                ProjectHub.Initialize();
            }
            catch (Exception Error)
            {
                Logging.Write("Error check logs!", ConsoleColor.DarkRed);
                Logging.LogError(Error.ToString());
            }

            while (true)
            {
                Logging.Write("");
                ConsoleCommandHandler.InvokeCommand(Console.ReadLine());
            }
        }
    }
}
