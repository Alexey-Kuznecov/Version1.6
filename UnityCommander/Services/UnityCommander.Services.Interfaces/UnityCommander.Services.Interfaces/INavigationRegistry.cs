
using System;
using UnityCommander.Core.Navigation;

namespace UnityCommander.Services.Interfaces
{
    public interface INavigationRegistry
    {
        NavigationManager Get(Guid tabId);

        void Register(Guid tabId, NavigationManager navigation);

        bool Remove(Guid tabId);
    }
}
