
using UnityCommander.Search.Models;

namespace UnityCommander.Search.Filtering
{
    public sealed class SizeSearchFilter : ISearchFilter
    {
        private readonly long _size;
        private readonly SizeComparison _comparison;

        public SizeSearchFilter(
            SizeComparison comparison,
            long size)
        {
            _comparison = comparison;
            _size = size;
        }

        public bool Match(SearchItem item)
        {
            return _comparison switch
            {
                SizeComparison.GreaterThan =>
                    item.Size > _size,

                SizeComparison.GreaterThanOrEqual =>
                    item.Size >= _size,

                SizeComparison.LessThan =>
                    item.Size < _size,

                SizeComparison.LessThanOrEqual =>
                    item.Size <= _size,

                SizeComparison.Equal =>
                    item.Size == _size,

                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
