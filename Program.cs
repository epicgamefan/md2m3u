using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace md2m3u
{
    class Program
    {
        static void Main(string[] args)
        {
            // Set Command Line Options
            bool optionAll = false;
            bool optionRecursive = false;
            foreach(var arg in args)
            {
                if(arg == "all")
                {
                    optionAll = true;
                }
                else if(arg == "recursive")
                {
                    optionRecursive = true;
                }
            }

            // Scan Directory Recursively For chd/iso/cue Image Files
            FileScanner scanner = new FileScanner();
            scanner.SearchFolder = ".";
            scanner.AllowedExtensions = new[] { ".chd", ".iso", ".cue", ".cdi", ".gdi", ".rvz", ".d64" };
            scanner.Recursive = optionRecursive;
            List<string> discImages = scanner.Scan();

            System.Console.WriteLine("Reading Games");
            // Turn Disc Images paths into Game objects.
            List<Game> games = new List<Game>();
            foreach(var di in discImages)
            {
                Game g = new Game(di);
                games.Add(g);
            }

            System.Console.WriteLine("Combining Multi Disc/Disk Games");
            // Combine multi disc games
            List<Game> processedGames = Game.CombineGames(games);

            System.Console.WriteLine("Writing m3u Files");
            // Write m3u files
            foreach (var g in processedGames)
            {
                if(g.DiscCount > 1 || optionAll)
                {
                    File.WriteAllText(g.M3uFile, g.M3uContent);
                }
            }
        }
    }
}
