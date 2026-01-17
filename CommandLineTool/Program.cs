using System;
using System.Collections.Generic;

namespace CommandLineTool
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // Argument check
            if (args.Length != 2)
            {
                WriteLineColor(Console.Error, ConsoleColor.DarkYellow,
                    $"[X] usage: {System.AppDomain.CurrentDomain.FriendlyName}.exe <folder_path> <json_server_url>");
                
                WriteLineColor(Console.Error, ConsoleColor.Yellow,
                    $"[!] e.g  : {System.AppDomain.CurrentDomain.FriendlyName}.exe C:/Users/user/Desktop/Gurum http://localhost:3000");

                return; // exiting the program
            }
            
            string folderPath = Path.GetFullPath(args[0]);
            string jsonPath = args[1];
            WriteLineColor(Console.Out, ConsoleColor.Green, $"[OK] Arguments passed successfully...");
            
            
            if (!Directory.Exists(folderPath))
            {
                WriteLineColor(Console.Error, ConsoleColor.DarkYellow, $"[X] Directory \"{folderPath}\" does not exist!");
                return;
            }
            WriteLineColor(Console.Out, ConsoleColor.Green, $"[OK] Directory exists...");
            
            IEnumerable<string> files = Directory.EnumerateFiles(folderPath).Where(s => s.EndsWith("map.png") || s.EndsWith("map.txt"));
            if (files.Count() != 2)
            {
                WriteLineColor(Console.Error, ConsoleColor.DarkYellow, $"[X] Directory \"{folderPath}\" does not contains map.png nor map.txt!");
                return;
            }
            WriteLineColor(Console.Out, ConsoleColor.Green, $"[OK] Files map.png and map.txt found in the directory...");
            
        }

        private static void CreateJson()
        {
            
        }

        private static void WriteLineColor(TextWriter tw, ConsoleColor color, string s)
        {
            Console.ForegroundColor = color;
            tw.WriteLine(s);
            Console.ResetColor();
        }
    }   
}