namespace Command_Line_Tool
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.ResetColor();
            if (args.Length != 2)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Error.WriteLine($"usage: {System.AppDomain.CurrentDomain.FriendlyName}.exe <folder_path> <json_server_url>");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Error.WriteLine($"e.g  : {System.AppDomain.CurrentDomain.FriendlyName}.exe C:/Users/user/Desktop/Gurum http://localhost:3000 ");
                Console.ResetColor();

                return;
            }

            string folderPath = Path.GetFullPath(args[0]);
            string jsonPath = args[1];
            
            if (!Directory.Exists(folderPath))
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Error.WriteLine($"Directory \"{folderPath}\" does not exist. ");
                Console.ResetColor();

                return;
            }
            
            IEnumerable<string> files = Directory.EnumerateFiles(folderPath).Where(s => s.EndsWith("map.png") || s.EndsWith("map.txt"));
            if (files.Count() != 2)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Error.WriteLine($"Directory \"{folderPath}\" does not contains map.png nor map.txt ");
                Console.ResetColor();

                return;
            }
        }
    }   
}