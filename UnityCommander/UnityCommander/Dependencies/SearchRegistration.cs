
using Prism.Ioc;
using UnityCommander.Search.Abstractions;
using UnityCommander.Search.Engine;
using UnityCommander.Search.Enumeration;
using UnityCommander.Services;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Dependencies
{
    public static class SearchRegistration
    {
        public static void Register(IContainerRegistry registry)
        {
            registry.RegisterSingleton<ISearchService, SearchService>();
            registry.RegisterSingleton<ISearchEngine, SearchEngine>();
            registry.RegisterSingleton<ISearchEnumerator, FileSystemSearchEnumerator>();
        }
    }
}
