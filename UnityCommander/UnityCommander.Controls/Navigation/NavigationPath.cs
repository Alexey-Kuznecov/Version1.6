
using System.Collections.Generic;

namespace UnityCommander.Controls.Navigation
{
    public sealed class NavigationPath
    {
        public IReadOnlyList<NavigationPathItem> Items { get; }

        public NavigationPath(
            IReadOnlyList<NavigationPathItem> items)
        {
            Items = items;
        }
    }
}
