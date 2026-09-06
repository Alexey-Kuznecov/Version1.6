
using System;
using System.Collections.Generic;

namespace UnityCommander.Modules.FilePanel.Services
{
    public class ViewportService<T>
    {
        private readonly Func<IReadOnlyList<T>> _source;

        private int _start;
        private int _end;

        public ViewportService(Func<IReadOnlyList<T>> source)
        {
            _source = source;
        }

        public void SetRange(int start, int end)
        {
            _start = start;
            _end = end;
        }

        public IEnumerable<T> GetVisibleItems()
        {
            var items = _source();

            for (int i = _start; i < _end && i < items.Count; i++)
                yield return items[i];
        }
    }
}
