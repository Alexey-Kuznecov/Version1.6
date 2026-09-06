
using System.Collections.Generic;
using System.Reflection;
using System.Windows;

namespace UnityCommander.Services.Interfaces.Plugins
{
    public interface IResourceLoader
    {
        IReadOnlyList<ResourceDictionary> Load(Assembly assembly);
    }
}
