
using IconMaker.Core.Storage;
using System.Collections.Generic;
using System.IO;

namespace IconBrowser.Services.Search
{
    public class IconIndexBuilder
    {
        public readonly IIconStorage _serializer;

        public IconIndexBuilder(IIconStorage serializer)
        {
            _serializer = serializer;
        }

        public IEnumerable<IconSearchResult> Build(string rootPath)
        {
            var results = new List<IconSearchResult>();

            foreach (var file in Directory.EnumerateFiles(rootPath, "*.json"))
            {
                var pack = _serializer.Load(Path.GetFileNameWithoutExtension(file));

                foreach (var icon in pack.Icons)
                {
                    results.Add(new IconSearchResult
                    {
                        Definition = icon,
                        IconId = icon.Id,
                        Name = icon.Name,
                        FilePath = file,
                        PackId = pack.Name
                    });
                }
            }

            return results;
        }

        private string GetPackId(string file) => Directory.GetParent(file)?.Name;
    }
}
