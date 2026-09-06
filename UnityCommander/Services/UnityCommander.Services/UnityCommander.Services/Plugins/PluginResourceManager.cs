
using PluginSystem.Runtime;
using System.Collections.Generic;
using System.Windows;
using UnityCommander.Services.Interfaces.Plugins;

namespace UnityCommander.Services.Plugins
{
    //public class PluginResourceManager : IPluginResourceManager
    //{
    //    private readonly Dictionary<string, IReadOnlyList<ResourceDictionary>> _resources = new();
        
    //    private IResourceLoader _resourceLoader;

    //    public PluginResourceManager(IResourceLoader resourceLoader)
    //    {
    //        _resourceLoader = resourceLoader;
    //    }

    //    public void Load(PluginContainer container)
    //    {
    //        if (_resources.ContainsKey(container.PluginID))
    //            return;

    //        var dictionaries = _resourceLoader.Load(container.LoadedAssembly);

    //        foreach (var dictionary in dictionaries)
    //        {
    //            Application.Current.Resources
    //                .MergedDictionaries
    //                .Add(dictionary);
    //        }

    //        _resources.Add(container.PluginID, dictionaries);
    //    }

    //    public void Unload(PluginContainer container)
    //    {
    //        if (!_resources.TryGetValue(container.PluginID, out var dictionaries))
    //            return;

    //        foreach (var dictionary in dictionaries)
    //        {
    //            Application.Current.Resources
    //                .MergedDictionaries
    //                .Remove(dictionary);
    //        }

    //        _resources.Remove(container.PluginID);
    //    }
    //}
}
