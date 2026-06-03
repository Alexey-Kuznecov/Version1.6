
using System.Collections.Generic;

namespace UnityCommander.Core.Theming
{
    public class ThemeDefinition
    {
        public string Name { get; set; }

        public IReadOnlyList<string> ResourceUris { get; set; }
    }
}