using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace md2m3u
{
    internal class FileScanner
    {

        private string[] _allowedExtensions = new[] { ".chd", ".iso", ".cue" };
        public string[] AllowedExtensions
        {
            get
            {
                return _allowedExtensions;
            }
            set
            {
                _allowedExtensions = value;
            }
        }

        private string _searchFolder = ".";
        public string SearchFolder
        {
            get
            {
                return _searchFolder;
            }
            set
            {
                _searchFolder = value;
            }
        }

        private SearchOption _searchOption = SearchOption.TopDirectoryOnly;
        public bool Recursive
        {
            get
            {
                return (_searchOption == SearchOption.AllDirectories);
            }
            set
            {
                if(value)
                {
                    _searchOption = SearchOption.AllDirectories;
                }
                else
                {
                    _searchOption = SearchOption.TopDirectoryOnly;
                } 
            }
        }

        public FileScanner()
        {
        }

        internal List<string> Scan()
        {
            
            List<string> discImages = Directory
                .GetFiles(_searchFolder, "*", _searchOption)
                .Where(file => _allowedExtensions.Any(file.ToLower().EndsWith))
                .ToList();

            return discImages;

        }
    }
}