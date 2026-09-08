
using System;
using System.Threading.Tasks;
using UnityCommander.Core.Navigation;
using UnityCommander.Modules.LeftSideBars.Widget.Contexts;
using UnityCommander.UI.Interaction;

namespace UnityCommander.Modules.LeftSideBars.Widget.Actions
{
    public sealed class OpenInPanelAction : IContextAction
    {
        private readonly NavigationManager _navigation;
        private readonly Guid _panelId;
        private readonly string _path;
        private readonly string _panelName;

        public string Id => $"open.in.panel.{_panelId}";

        public string Name => $"Панель {_panelName}";

        public OpenInPanelAction(
            NavigationManager navigation,
            Guid panelId,
            string panelName,
            string path)
        {
            _navigation = navigation;
            _panelId = panelId;
            _panelName = panelName;
            _path = path;
        }

        public bool CanExecute(IInteractionContext context)
            => context is DiskContext;

        public Task ExecuteAsync(IInteractionContext context)
        {
            _navigation.NavigateTo(_path);
            return Task.CompletedTask;
        }
    }
}
