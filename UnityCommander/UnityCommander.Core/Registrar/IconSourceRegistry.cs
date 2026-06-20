
using System.Collections.Generic;
using UnityCommander.Abstractions.Resources;

namespace UnityCommander.Core.Registrar
{
    public class IconSourceRegistry : IIconSourceRegistry
    {
        public readonly List<IIconSource> _sources 
            = new List<IIconSource>();

        public IReadOnlyCollection<IIconSource> Sources => _sources;

        public void Register(IIconSource source)
        {
            _sources.Add(source);
        }
    }
}
