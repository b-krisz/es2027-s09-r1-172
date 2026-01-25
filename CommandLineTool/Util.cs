using System;
using System.IO;

namespace CommandLineTool
{
    internal static class Util
    {
        internal static void WriteLineColor(TextWriter tw, ConsoleColor color, string s)
        {
            Console.ForegroundColor = color;
            tw.WriteLine(s);
            Console.ResetColor();
        }
        internal static void WriteColor(TextWriter tw, ConsoleColor color, string s)
        {
            Console.ForegroundColor = color;
            tw.Write(s);
            Console.ResetColor();
        }
    }
}

    