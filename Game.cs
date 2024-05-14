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
        private string _multiDiscType = "";
        private string _multiDiscLabel = "";

        private static Dictionary<string, string> _regexPatterns = new Dictionary<string, string>()
        {
            { "redump", @"^.*?( \(Disc ([0-9])\))[^$]*$" },
            { "disc", @"^.*?( \(Disc([0-9])\))[^$]*$" },
            { "dvd", @"^.*?( - DVD-([0-9]))[^$]*$" },
            { "cd", @"^.*?( - CD([0-9]))[^$]*$" },
            { "disk", @"^.*?( \(Disk ([0-9]).*?(Side ([AB]))?\))[^$]*$" },
            { "side", @"^.*?( \(Side ([AB]).*?\))[^$]*$" },
        };

        private SortedDictionary<string, string> _discImages = new SortedDictionary<string, string>();
        private SortedDictionary<string, string> _discLabels = new SortedDictionary<string, string>();

        public SortedDictionary<string, string> DiscImages
        {
            get { return _discImages; }
        }

        public SortedDictionary<string, string> DiscLabels
        {
            get { return _discLabels; }
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
                    string label = DiscLabels[di.Key];
                    m3uContent += Path.GetFileName(di.Value);
                    if (label != "")
                    {
                       // m3uContent += "|" + label;
                    }
                    m3uContent += "\n";

                }

                return m3uContent;
            }
        }

        private static string ParseGameName(string discImage)
        {
            return Path.GetFileNameWithoutExtension(discImage) + " (" + Path.GetExtension(discImage).ToUpper().Remove(0, 1) + ")";
        }

        private static string ParseDiscNumber(string discImage)
        {
            string gameName = Game.ParseGameName(discImage);

            foreach (var regexPattern in _regexPatterns)
            {
                Match m = Regex.Match(gameName, regexPattern.Value, RegexOptions.None);
                if (m.Success)
                {
                    
                    if (m.Groups.Count >= 4)
                    {
                        return int.Parse(m.Groups[2].Value).ToString() + m.Groups[4].Value.ToString();
                    }
                    else if (m.Groups.Count >= 2 && (regexPattern.Key != "side"))
                    {
                        return int.Parse(m.Groups[2].Value).ToString();
                    }
                    else if (m.Groups.Count >= 2 && (regexPattern.Key == "side"))
                    {
                        return m.Groups[2].Value.ToString();
                    }
                }
            }
            return "0";
        }

        public Game(string discImage)
        {
            _gameExt = Path.GetExtension(discImage);
            _gameDir = Path.GetDirectoryName(discImage);
            _gameName = Game.ParseGameName(discImage);


            foreach (var regexPattern in _regexPatterns)
            {
                Match m = Regex.Match(_gameName, regexPattern.Value, RegexOptions.None);
                if (m.Success)
                {
                    if (_gameExt.ToLower() == ".d64")
                    {
                        string[] s = discImage.Split(new string[] { m.Groups[1].Value }, System.StringSplitOptions.None);
                        _gameName = s[0];
                        _multiDiscType = regexPattern.Key;

                        if (s.Length >= 2)
                        {
                            string labelregex = @"^.*?\((.*)?\)[^$]*$";
                            Match labelMatch = Regex.Match(s[1], labelregex, RegexOptions.None);
                            if (labelMatch.Success)
                            {
                                _multiDiscLabel = labelMatch.Groups[1].Value;
                                

                            }
                        }
                        
                    }
                    else
                    {
                        _gameName = _gameName.Replace(m.Groups[1].Value, "");
                        _multiDiscType = regexPattern.Key;
                        if (_multiDiscType != "redump")
                        {
                            _gameName += " (" + _multiDiscType.ToUpper() + ")";
                        }
                    }

                    break;
                }
            }

            string num = Game.ParseDiscNumber(discImage);
            _discImages.Add(num, discImage);
            _discLabels.Add(num, _multiDiscLabel);

            /*            System.Console.WriteLine(discImage);
                        System.Console.WriteLine(_gameExt);
                        System.Console.WriteLine(_gameDir);
                        System.Console.WriteLine(_gameName);
                        System.Console.WriteLine(_multiDiscLabel);
             */

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

            if (g._multiDiscType != _multiDiscType)
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
                        string num = Game.ParseDiscNumber(di.Value);
                        if (!_discImages.ContainsKey(num))
                        {
                            _discImages.Add(num, di.Value);
                            _discLabels.Add(num, g._multiDiscLabel);
                        }
                        else 
                        {
                            System.Console.WriteLine("Error: Duplicate Disk Number Found in " + di);
                            System.Console.WriteLine("Skipping it");
                        }
                    }
                    else
                    {
                        System.Console.WriteLine("Error: Duplicate Disk Image Found with " + di);
                        System.Console.WriteLine("Skipping it");
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