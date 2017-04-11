using System;
using ProjectHub.Core;
using System.Security.Permissions;
using System.Runtime.InteropServices;
using System.Net;
using System.IO;

namespace ProjectHub
{
    class Program
    {
        private const int MF_BYCOMMAND = 0x00000000;
        public const int SC_CLOSE = 0xF060;

        [DllImport("user32.dll")]
        public static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlAppDomain)]

        static void Main(string[] args)
        {
            DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_CLOSE, MF_BYCOMMAND);

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
