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
            // Scan Directory Recursively For chd/iso/cue Image Files
            FileScanner scanner = new FileScanner();
            scanner.SearchFolder = "E:\\chd_psx\\CHD-PSX-Improvements";
            scanner.AllowedExtensions = new[] { ".chd", ".iso", ".cue" };
            scanner.Recursive = true;
            List<string> discImages = scanner.Scan();

            // Turn Disc Images paths into Game objects.
            List<Game> games = new List<Game>();
            foreach(var di in discImages)
            {
                Game g = new Game(di);
                games.Add(g);
            }

            // Combine multi disc games
            List<Game> processedGames = Game.CombineGames(games);

            // Write m3u files
            foreach (var g in processedGames)
            {
                if(g.DiscCount > 1)
                {
                    File.WriteAllText(g.M3uFile, g.M3uContent);
                }
            }
        }
    }
}
