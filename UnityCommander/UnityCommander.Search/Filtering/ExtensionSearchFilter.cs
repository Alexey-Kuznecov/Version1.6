
using System.IO;
using UnityCommander.Search.Models;

namespace UnityCommander.Search.Filtering
{
    public sealed class ExtensionSearchFilter : ISearchFilter
    {
        private readonly HashSet<string> _extensions;

        public ExtensionSearchFilter(IEnumerable<string> extensions)
        {
            _extensions = new HashSet<string>(
                extensions.Select(Normalize),
                StringComparer.OrdinalIgnoreCase);
        }

        public bool Match(SearchItem item)
        {
            if (item.IsDirectory)
                return false;

            return _extensions.Contains(
                Path.GetExtension(item.Path));
        }

        private static string Normalize(string extension)
        {
            return extension.StartsWith('.')
                ? extension
                : "." + extension;
        }
    }
}
