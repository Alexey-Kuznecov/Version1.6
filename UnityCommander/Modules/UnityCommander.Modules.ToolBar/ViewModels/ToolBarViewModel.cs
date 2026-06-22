

namespace UnityCommander.Modules.ToolBar.ViewModels
{
    using CommandSystem.Abstractions;
    using Prism.Dialogs;
    using Prism.Mvvm;
    using System;
    using System.Threading.Tasks;
    using System.Windows.Shapes;
    using UnityCommander.Abstractions.Command;
    using UnityCommander.Abstractions.Dialog;
    using UnityCommander.Abstractions.Ribbon;
    using UnityCommander.Common.Commands;
    using UnityCommander.Common.State;
    using UnityCommander.Core.Commands;
    using UnityCommander.Core.Plugin;
    using UnityCommander.Modules.ToolBar.Builder;
    using UnityCommander.Ribbon.Abstractions.Models;
    using UnityCommander.Ribbon.Services;
    using UnityCommander.Ribbon.Services.Wpf;
    using UnityCommander.Services;
    using UnityCommander.Services.Interfaces.Settings;
    using UnityCommander.Services.Layout;

    /// <summary>
    /// The view a view model.
    /// </summary>
    public class ToolBarViewModel : BindableBase
    {
        #region Dependency Injection Fields

        private IShellLayoutManager _shellLayoutManager;
        private IRibbonRegistry _ribbonRegistry;

        #endregion

        public IRibbonManager RibbonManager { get; }

        public ToolBarViewModel(
            ISettingsProviderService configService,
            IRibbonRegistry ribbonRegistry,
            IRibbonManager ribbonManager,
            IShellLayoutManager shellLayoutManager,
            CommandExecutionService commandExecution, 
            CommandRegistryService commandRegistry,
            IRibbonBindingRegistry bindingRegistry, 
            IPluginCommandRegistry pluginCommand,
            IRibbonModelFactory modelFactory,
            IRibbonCommandResolver resolver)
        {
            _ribbonRegistry = ribbonRegistry;
            _shellLayoutManager = shellLayoutManager;
            RibbonManager = ribbonManager;
            IsExpanded = true;

            RibbonManager.Configure(new RibbonServices()
            {
                Commands = resolver
            });

            RibbonManager.TabCollapsed += RibbonManager_TabCollapsed;
            RibbonManager.TabExpanded += RibbonManager_TabExpanded;

            commandRegistry.Register(CommandFactoryExtensions.Create(
                 CommandNames.UI.ToggleRibbon,
                 ToggleRibbon
             ));

            ConfigureRibbon(r =>
            {
                r.Tab("home", "Главная")
                    .Group("tools", "Инструменты")
                        .Section("main", RibbonGroupLayout.Large)
                            .Button(CommandNames.UI.ToggleBottomPanel, "core.drive");
            });

            var ribbon = modelFactory.Create();

            RibbonManager.SetModel(ribbon);
        }

        public void ConfigureRibbon(
          Action<RibbonBuilder> configure)
        {
            var ribbon = new RibbonDefinition();

            var builder = new RibbonBuilder(ribbon);

            configure(builder);

            _ribbonContribution =
                new RibbonContribution(
                    "core.owner",
                    ribbon);

            _ribbonRegistry.Register(_ribbonContribution);
        }

        private void RibbonManager_TabCollapsed(object sender, RibbonTabEventArgs e)
        {
            _shellLayoutManager.SetState(
                ShellArea.Ribbon,
                new ShellAreaState
                {
                    Size = 38
                });
        }

        private void RibbonManager_TabExpanded(object sender, RibbonTabEventArgs e)
        {
            _shellLayoutManager.SetState(
                 ShellArea.Ribbon,
                 new ShellAreaState
                 {
                     Size = 180
                 });
        }

        private bool _isRibbonExpanded;
        private RibbonContribution _ribbonContribution;

        public bool IsExpanded
        {
            get => _isRibbonExpanded;
            set
            {
                if (!SetProperty(ref _isRibbonExpanded, value))
                    return;
            }
        }

        public Task ToggleRibbon(CommandContext ctx)
        {
            IsExpanded = !IsExpanded;

            UpdateLayout();

            return Task.CompletedTask;
        }

        internal void Capture(AppSessionState state)
        {
            state.Ribbon.IsExpanded = IsExpanded;
        }

        internal void Restore(AppSessionState state)
        {
            IsExpanded = state.Ribbon.IsExpanded;

            UpdateLayout();
        }

        private void UpdateLayout()
        {
            _shellLayoutManager.SetState(
                ShellArea.Ribbon,
                new ShellAreaState
                {
                    Size = IsExpanded ? 180 : 0
                });
        }
    }
}
