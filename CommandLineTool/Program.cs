using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

// TODO: UNIT TESTING
// TODO: MORE EXPLICIT ERROR HANDLING, AND ERROR MESSAGES

namespace CommandLineTool
{
    internal static class Program
    {
        private static readonly HttpClient HttpClient = new HttpClient();
        private static void Main(string[] args)
        {
            // Argument check
            if (args.Length != 2)
            {
                Util.WriteLineColor(Console.Error, ConsoleColor.DarkYellow,
                    $"[X] usage: {AppDomain.CurrentDomain.FriendlyName}.exe <folder_path> <json_server_url>");
                
                Util.WriteLineColor(Console.Error, ConsoleColor.Yellow,
                    $"[!] e.g  : {AppDomain.CurrentDomain.FriendlyName}.exe C:/Users/user/Desktop/Gurum http://localhost:3000/projects");

                return; // exiting the program
            }
            Util.WriteLineColor(Console.Out, ConsoleColor.Green, "[OK] Arguments passed successfully...");
            
            
            string folderPath = Path.GetFullPath(args[0]) + '\\';
            string jsonUrl = args[1];
            Project? project = null;

            
            if (TryLoadLockFile(out string data)) // If we are able to retrieve a lockfile, giving a chance to use it, it creates a project from it 
            {
                Util.WriteColor(Console.Out, ConsoleColor.DarkGreen, $"[!] Found lockfile... Do you want to open it and resume the last session? (y/n): ");
                if (Console.ReadKey().Key == ConsoleKey.Y)
                {
                    Console.WriteLine();
                    project = JsonSerializer.Deserialize<Project>(data);
                }
                else
                {
                    Console.WriteLine();
                    Util.WriteLineColor(Console.Out, ConsoleColor.DarkGreen, $"[!] Skipping lockfile...");
                }
            }
            
            if (project == null) // If it's still null, the user skipped the lockfile
            {
                string[]? files = LoadFiles(folderPath);
                if (files == null) return;
        
            
                project = CreateProject(folderPath, files);
                if (project == null) return;

                // Trying to a session into a lockfile
                if (TrySaveLockFile(JsonSerializer.Serialize(project)))
                    Util.WriteLineColor(Console.Out, ConsoleColor.Green, "[OK] Successfully saved lockfile...");
                else Util.WriteLineColor(Console.Error, ConsoleColor.DarkYellow, "[x] Failed to save lockfile!");
            }


            if (TryUploadJson(jsonUrl, JsonSerializer.Serialize(project)).Result)
            {
                Util.WriteLineColor(Console.Out, ConsoleColor.Green, "[OK] Successfully uploaded JSON...");
                if (TryDeleteLockFile())
                    Util.WriteLineColor(Console.Out, ConsoleColor.DarkGreen, "[!] Successfully deleted lockfile...");
                else
                    Util.WriteLineColor(Console.Error, ConsoleColor.DarkYellow, "[x] Failed to delete lockfile!");
            }
            else
                Util.WriteLineColor(Console.Error, ConsoleColor.DarkYellow, "[x] Failed to upload JSON!");
        }
        
        private static string[]? LoadFiles(string folderPath)
        {
            // Checking if the directory exists
            if (!Directory.Exists(folderPath))
            {
                Util.WriteLineColor(Console.Error, ConsoleColor.DarkYellow, $"[X] Directory \"{folderPath}\" does not exist!");
                return null;
            }
            Util.WriteLineColor(Console.Out, ConsoleColor.Green, $"[OK] Directory exists...");
            
            
            // Ordering the files, for easier indices
            string[] files = Directory.EnumerateFiles(folderPath).Where(s => s.EndsWith("map.png") || s.EndsWith("map.txt")).OrderBy(x => x).ToArray();
            
            
            // After filtering out the files for map.png and map.txt, if the Length is not 2, its obvious that one of them is missing
            if (files.Length != 2)
            {
                Util.WriteLineColor(Console.Error, ConsoleColor.DarkYellow, $"[X] Directory \"{folderPath}\" does not contains map.png nor map.txt!");
                return null;
            }
            Util.WriteLineColor(Console.Out, ConsoleColor.Green, $"[OK] Files map.png and map.txt found in the directory...");

            return files;
        }
        private static Project? CreateProject(string folderPath, string[] filePaths)
        {
            string projectName = Path.GetFileName(Path.GetDirectoryName(folderPath)) ?? "ProjectName";
            string projectMapData = "data:image/png;base64,"+Convert.ToBase64String(File.ReadAllBytes(filePaths[0]));
            Cell[] projectCells = new Cell[400]; // 20x20 grid
            try
            {
                using (FileStream fs = new FileStream(filePaths[1], FileMode.Open))
                {
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        string[] lines = sr.ReadToEnd().Split('\n');
                        for (int y = 0; y < 20; y++)
                        {
                            for (int x = 0; x < 20; x++)
                            {
                                string type = lines[y][x] switch
                                {
                                    'O' => "Grass",
                                    'X' => "Lake",
                                    '#' => "Mountain",
                                    _ => "UNKNOWN"
                                };
                                projectCells[y * 20 + x] = new Cell(x, y, type, false);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                Util.WriteLineColor(Console.Error, ConsoleColor.DarkYellow, $"[X] Failed to create project!");
                return null;
            }
            Util.WriteLineColor(Console.Out, ConsoleColor.Green, $"[OK] {projectName} Project created...");
            
            return new Project(projectName, projectMapData, projectCells);
        }
        
        private static async Task<bool> TryUploadJson(string jsonUrl, string data)
        {
            try
            {
                HttpClient.BaseAddress = new Uri(jsonUrl);
                HttpResponseMessage response = await HttpClient.PostAsync("", new StringContent(data, Encoding.UTF8, "application/json"));
                response.EnsureSuccessStatusCode();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
        private static bool TryLoadLockFile(out string data)
        {
            data = "";
            try
            {
                if (!File.Exists("./output.lock")) return false;
                using (FileStream fs = new FileStream("./output.lock", FileMode.Open))
                {
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        data = sr.ReadToEnd();
                    }
                }
            }
            catch (Exception )
            {
                return false;
            }

            return true;
        }
        private static bool TrySaveLockFile(string data)
        {
            try
            {
                using (FileStream fs = new FileStream("./output.lock", FileMode.Create))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        sw.Write(data);
                    }
                }
            }
            catch (Exception )
            {
                return false;
            }

            return true;
        }
        private static bool TryDeleteLockFile()
        {
            try
            {
                File.Delete("./output.lock");
            }
            catch (Exception )
            {
                return false;
            }

            return true;
        }
        
    }   
}