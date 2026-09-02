
using UnityCommander.Search.Models;

namespace UnityCommander.Search.Filtering
{
    public sealed class ModifiedAfterFilter : ISearchFilter
    {
        private readonly DateTime _date;

        public ModifiedAfterFilter(DateTime date)
        {
            _date = date;
        }

        public bool Match(SearchItem item)
        {
            return item.LastWriteTime >= _date;
        }
    }
}
