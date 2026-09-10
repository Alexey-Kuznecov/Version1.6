
using System;
using UnityCommander.Abstractions.Panels;
using UnityCommander.Common.Panels;
using UnityCommander.Core.Navigation;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Services 
{
    public sealed class ActiveNavigationService
    {
        private readonly ActiveTabContext _activeTab;
        private readonly IPanelRegistry _panels;
        private readonly INavigationRegistry _navigationRegistry;

        public ActiveNavigationService(
            IPanelRegistry panels,
            INavigationRegistry navigationRegistry, 
            ActiveTabContext activeTab)
        {
            _panels = panels;
            _navigationRegistry = navigationRegistry;
            _activeTab = activeTab;
        }

        public void Navigate(string path)
        {
            var navigation = GetActiveNavigation();
            navigation?.TryNavigateTo(path);
        }

        public void GoParent()
        {
            var navigation = GetActiveNavigation();
            if (navigation?.CanGoParent == true)
                navigation.GoParent();
        }

        public void GoBack()
        {
            var navigation = GetActiveNavigation();
            if (navigation?.CanGoBack == true)
                navigation.GoBack();
        }

        public void GoForward()
        {
            var navigation = GetActiveNavigation();
            if (navigation?.CanGoForward == true)
                navigation.GoForward();
        }

        private NavigationManager? GetActiveNavigation()
        {
            //var panel = _panels.GetActivePanel();
            //if (panel?.ActiveTabId is not Guid tabId)
            //    return null;

            return _navigationRegistry.Get(_activeTab.ActiveTabId);
        }
    }
}
