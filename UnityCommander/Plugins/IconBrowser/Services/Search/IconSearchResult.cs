
using IconMaker.Core.Models;
using System;

namespace IconBrowser.Services.Search
{
    public class IconSearchResult
    {
        public IconDefinition Definition { get; set;  }

        public Guid IconId { get; set; }
        public string Name { get; set; }

        public string FilePath { get; set; }
        public string PackId { get; set; }
    }
}
