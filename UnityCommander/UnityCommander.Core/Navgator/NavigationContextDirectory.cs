
using System;
using System.Collections.Generic;

namespace UnityCommander.Core.Navigation
{
    // ---------------------------
    // Глобальный реестр контекстов
    // ---------------------------
    public class NavigationContextDirectory
    {
        private readonly Dictionary<Guid, NavigationManager> _map = new();

        private static NavigationContextDirectory? _instance;
        public static NavigationContextDirectory Instance => _instance ??= new NavigationContextDirectory();

        public NavigationContextDirectory() { }

        public NavigationManager Get(Guid panelId)
        {
            if (!_map.TryGetValue(panelId, out var nav))
            {
                nav = new NavigationManager();
                _map[panelId] = nav;
            }
            return nav;
        }

        public void Register(Guid panelId, NavigationManager nav)
        {
            _map[panelId] = nav;
        }
    }
}
