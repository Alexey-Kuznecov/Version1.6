
using CommandSystem.Abstractions;
using Prism.Mvvm;
using System.Threading.Tasks;
using UnityCommander.Common.Commands;
using UnityCommander.Common.State;
using UnityCommander.Core.Commands;
using UnityCommander.Services;
using UnityCommander.Services.Layout;

namespace UnityCommander.Modules.BottomPanel.ViewModels
{
    public class BottomPanelViewModel : BindableBase
    {
        private IShellLayoutManager _shellLayoutManager;

        private bool _isBottonPanelOpen;

        public BottomPanelViewModel(
            IShellLayoutManager shellLayoutManager,
           CommandRegistryService commandRegistry)
        {
            _shellLayoutManager = shellLayoutManager;

            commandRegistry.Register(CommandFactoryExtensions.Create(
                 CommandNames.UI.ToggleBottomPanel,
                 ToggleBottom
            ));
        }

        public bool IsBottonPanelOpen
        {
            get => _isBottonPanelOpen;
            set
            {
                if (!SetProperty(ref _isBottonPanelOpen, value))
                    return;
                UpdateLayout();
            }
        }

        public Task ToggleBottom(CommandContext ctx)
        {
            IsBottonPanelOpen = !IsBottonPanelOpen;

            UpdateLayout();

            return Task.CompletedTask;
        }

        internal void Capture(AppSessionState state)
        {
            state.BottomPanel.IsOpen = IsBottonPanelOpen;
        }

        internal void Restore(AppSessionState state)
        {
            IsBottonPanelOpen = state.BottomPanel.IsOpen;
        }

        private void UpdateLayout()
        {
            _shellLayoutManager.SetState(
                ShellArea.BottomPanel,
                new ShellAreaState
                {
                    Size = IsBottonPanelOpen ? 250 : 0
                });
        }
    }
}
 