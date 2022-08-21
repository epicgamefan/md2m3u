using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace md2m3u
{


    internal class Game
    {
        private string _gameName = "";
        private string _gameDir = "";
        private string _gameExt = "";
        private static string _regexPattern = @"^.*?(\s?\(Disc\s?([0-9])\))[^$]*$";
        private SortedDictionary<int, string> _discImages = new SortedDictionary<int, string>();

        public SortedDictionary<int, string> DiscImages
        {
            get { return _discImages; }
        }

        public string Name
        {
            get { return _gameName; }
        }

        public string Dir
        {
            get { return _gameDir; }
        }

        public string Ext
        {
            get { return _gameExt; }
        }

        public int DiscCount
        {
            get
            {
                return _discImages.Count;
            }
        }

        public string M3uFile
        {
            get { return Dir + Path.DirectorySeparatorChar + Name + ".m3u"; }
        }

        public string M3uContent
        {
            get
            {
                string m3uContent = "";
                foreach (var di in DiscImages)
                {
                    m3uContent += Path.GetFileName(di.Value) + "\n";
                }

                return m3uContent;
            }
        }

        public static string ParseGameName(string discImage)
        {
            return Path.GetFileNameWithoutExtension(discImage) + " (" + Path.GetExtension(discImage).ToUpper().Remove(0, 1) + ")";
        }

        public static int ParseDiscNumber(string discImage)
        {
            string gameName = Game.ParseGameName(discImage);

            Match m = Regex.Match(gameName, _regexPattern, RegexOptions.None);
            if (m.Success)
            {
                return int.Parse(m.Groups[2].Value);
            }
            return 0;
        }

        public Game(string discImage)
        {
            _gameExt = Path.GetExtension(discImage);
            _gameDir = Path.GetDirectoryName(discImage);

            _discImages.Add(Game.ParseDiscNumber(discImage), discImage);
            _gameName = Game.ParseGameName(discImage); 

            Match m = Regex.Match(_gameName, _regexPattern, RegexOptions.None);
            if (m.Success)
            {
                _gameName = _gameName.Replace(m.Groups[1].Value, "");
            }
        }

        public bool IsGameCombinable(Game g)
        {
            if(g._gameName != _gameName)
            {
                return false;
            }

            if (g._gameExt != _gameExt)
            {
                return false;
            }

            if (g._gameDir  != _gameDir)
            {
                return false;
            }

            return true;
        }

        public void CombineGame(Game g)
        {
            if(IsGameCombinable(g))
            {
                foreach(var di in g._discImages)
                {
                    if (!_discImages.ContainsValue(di.Value))
                    {
                        _discImages.Add(Game.ParseDiscNumber(di.Value), di.Value);
                    }
                }
            }
        }

        public static List<Game> CombineGames(List<Game> games)
        {
            List<Game> processedGames = new List<Game>();
            foreach (var g in games)
            {
                bool combined = false;
                foreach (var pg in processedGames)
                {
                    if (pg.IsGameCombinable(g))
                    {
                        pg.CombineGame(g);
                        combined = true;
                        break;
                    }
                }

                if (!combined)
                {
                    processedGames.Add(g);
                }
            }

            return processedGames;
        }
    }
}