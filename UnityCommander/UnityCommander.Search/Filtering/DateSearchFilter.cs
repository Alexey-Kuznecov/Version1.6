
using UnityCommander.Search.Models;

namespace UnityCommander.Search.Filtering
{
    public sealed class DateSearchFilter : ISearchFilter
    {
        private readonly DateField _field;
        private readonly DateComparison _comparison;
        private readonly DateTime _date;

        public DateSearchFilter(
            DateField field,
            DateComparison comparison,
            DateTime date)
        {
            _field = field;
            _comparison = comparison;
            _date = date;
        }

        public bool Match(SearchItem item)
        {
            var value = _field switch
            {
                DateField.Creation => item.CreationTime,
                DateField.LastWrite => item.LastWriteTime,
                _ => throw new ArgumentOutOfRangeException()
            };

            return _comparison switch
            {
                DateComparison.After => value >= _date,
                DateComparison.Before => value < _date,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
