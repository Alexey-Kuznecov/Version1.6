

using System;
using System.Collections.Generic;
using System.Linq;

namespace IconBrowser.Services.Search
{
    public class IconSearchIndex
    {
        private IEnumerable<IconSearchResult> _data;

        public void Set(IEnumerable<IconSearchResult> data)
            => _data = data;

        public List<IconSearchResult> SearchRaw(string query)
        {
            return _data
                .Where(x =>
                    x.Name.StartsWith(
                        query,
                        StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
    }
}
