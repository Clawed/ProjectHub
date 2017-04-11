using System;
using System.IO;
using System.Text;

namespace ProjectHub.Core
{
    public static class Logging
    {
        internal static void WriteLine(string Text, ConsoleColor Colour = ConsoleColor.DarkYellow)
        {
            Console.ForegroundColor = Colour;
            Console.WriteLine("[" + DateTime.Now.ToString("HH:mm") + "] >> " + Text);
        }

        internal static void Write(string Text, ConsoleColor Colour = ConsoleColor.DarkYellow)
        {
            Console.ForegroundColor = Colour;
            Console.Write("[" + DateTime.Now.ToString("HH:mm") + "] >> " + Text);
        }

        internal static void WriteSimpleLine(string Text, ConsoleColor Colour = ConsoleColor.DarkYellow)
        {
            Console.ForegroundColor = Colour;
            Console.WriteLine(Text);
        }

        internal static void WriteSimple(string Text, ConsoleColor Colour = ConsoleColor.DarkYellow)
        {
            Console.ForegroundColor = Colour;
            Console.Write(Text);
        }

        internal static void LogError(string Text)
        {
            try
            {
                FileStream Writer = new FileStream(@"errors.txt", FileMode.Append, FileAccess.Write);
                byte[] Data = Encoding.ASCII.GetBytes(Environment.NewLine + Text + Environment.NewLine);
                Writer.Write(Data, 0, Data.Length);
                Writer.Dispose();
                WriteLine("Error occured, check errors.txt file for more information!", ConsoleColor.Red);
            }
            catch (Exception Error)
            {
                WriteLine("Could not write to file: " + Error + ":" + Text, ConsoleColor.Red);
            }
        }
    }
}