
using System;

namespace UnityCommander.Modules.FilePanel.Services
{
    public class ViewportMapper
    {
        public event Action<int, int>? RangeChanged;

        public void Update(double offset, double viewport)
        {
            int start = (int)offset;
            int end = start + (int)Math.Ceiling(viewport);

            RangeChanged?.Invoke(start, end + 1);
        }
    }
}
