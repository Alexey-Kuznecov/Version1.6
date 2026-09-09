
namespace UnityCommander.Modules.ToolBar.ViewModels
{
    using CommandSystem.Abstractions;
    using Prism.Mvvm;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using UnityCommander.Abstractions.Command;
    using UnityCommander.Abstractions.Ribbon;
    using UnityCommander.Common.Commands;
    using UnityCommander.Common.State;
    using UnityCommander.Core.Commands;
    using UnityCommander.Modules.ToolBar.Builder;
    using UnityCommander.Ribbon.Abstractions.Models;
    using UnityCommander.Ribbon.Services;
    using UnityCommander.Ribbon.Services.Wpf;
    using UnityCommander.Services;
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

            RibbonManager.TabCollapsed += RibbonManager_TabCollapsed;
            RibbonManager.TabExpanded += RibbonManager_TabExpanded;

            commandRegistry.Register(CommandFactoryExtensions.Create(
                 CommandNames.UI.ToggleRibbon,
                 ToggleRibbon
             ));

            ConfigureRibbon(r =>
            {
                r.Tab("home", "Главная")
                    .Group("clipboard", "Буфер обмена")
                        .Section("clip", RibbonGroupLayout.Inline)
                            .Button("Кнопка 1", CommandNames.Test.ShowParams, "directory.create2")
                            .Button("Кнопка 2", CommandNames.Test.ShowParams, "directory.create2")
                            .Button("Кнопка 3", CommandNames.Test.ShowParams, "directory.create2")

                        .EndSection()

                        .Section("clip2", RibbonGroupLayout.Medium)
                         .Button("Кнопка 1", CommandNames.Test.ShowDialog, "directory.create2")
                         .Button("Кнопка 2", CommandNames.Test.ShowDialog, "directory.create2")
                         .Button("Кнопка 3", CommandNames.Test.ShowDialog, "directory.create2")

                        .EndSection()
                    .EndGroup()
                     .Group("navigation", "Навигация")
                        .Section("nav", RibbonGroupLayout.Large)
                         .Button("Кнопка 1", CommandNames.Test.ShowParams, "directory.create2")
                         .Button("Кнопка 2", CommandNames.Test.ShowParams, "directory.create2")
                         .Button("Кнопка 3", CommandNames.Test.ShowParams, "directory.create2")
                         .Button("Кнопка 4", CommandNames.Test.ShowParams, "directory.create2")
                    .EndSection()
                       .Section("nav2", RibbonGroupLayout.Small)
                         .Button("Кнопка 1", CommandNames.Test.ShowParams, "directory.create2")
                         .Button("Кнопка 2", CommandNames.Test.ShowParams, "directory.create2")
                         .Button("Кнопка 3", CommandNames.Test.ShowParams, "directory.create2")
                         .Button("Кнопка 4", CommandNames.Test.ShowParams, "directory.create2")
                     .EndSection()
                     .EndGroup()
                      .Group("view", "Вид")
                       .Section("view", RibbonGroupLayout.Medium)
                         .ComboBox(CommandNames.Test.ShowDialog, new List<RibbonComboBoxItemDefinition>()
                         {
                             new RibbonComboBoxItemDefinition()
                             {
                                 CommandId = "1",
                                 IconKey = "directory.create2",
                                 Id = "2",
                                 Title = "directory"
                             },
                             new RibbonComboBoxItemDefinition()
                             {
                                 CommandId = "2",
                                 IconKey = "directory.create2",
                                 Id = "3",
                                 Title = "directory"
                             }
                         }
                        )
                         .CheckBox("check 1", "directory.create2")
                        .CheckBox("check 2", "directory.create2")
                          .EndSection()
                       .Section("view2", RibbonGroupLayout.Small)
                         .Button("Кнопка 1", CommandNames.Test.ShowParams, "directory.create2")
                         .Button("Кнопка 2", CommandNames.Test.ShowParams, "directory.create2")
                         .Button("Кнопка 3", CommandNames.Test.ShowParams, "directory.create2")
                         .Button("Кнопка 4", CommandNames.Test.ShowParams, "directory.create2");


                r.Tab("home", "Главная")
                  .Group("tools", "Интрументы")
                      .Section("editor", RibbonGroupLayout.Inline)
                         .Button("Кнопка 1", CommandNames.Test.ShowParams, "directory.create2")
                          .CheckBox("check 1", "directory.create2")
                      .EndSection()
                      .Section("converter", RibbonGroupLayout.Medium)
                         .Button("Кнопка 1", CommandNames.Test.ShowParams, "directory.create2")
                          .CheckBox("check 1", "directory.create2")
                          .ComboBox(CommandNames.Test.ShowParams, new List<RibbonComboBoxItemDefinition>()
                         {
                             new RibbonComboBoxItemDefinition()
                             {
                                 CommandId = "1",
                                 IconKey = "directory.create2",
                                 Id = "2",
                                 Title = "Item 4"
                             },
                             new RibbonComboBoxItemDefinition()
                             {
                                 CommandId = "2",
                                 IconKey = "directory.create2",
                                 Id = "3",
                                 Title = "Item 3"
                             }
                         }
                        )
                      .EndSection()
                        .Section("compress", RibbonGroupLayout.Small)
                         .Button("Кнопка 1", CommandNames.Test.ShowParams, "directory.create2")
                         .Button("Кнопка 2", CommandNames.Test.ShowParams, "directory.create2")
                        .ComboBox(CommandNames.Test.ShowParams, new List<RibbonComboBoxItemDefinition>()
                         {
                             new RibbonComboBoxItemDefinition()
                             {
                                 CommandId = "1",
                                 IconKey = "directory.create2",
                                 Id = "2",
                                 Title = "Item 1"
                             },
                             new RibbonComboBoxItemDefinition()
                             {
                                 CommandId = "2",
                                 IconKey = "directory.create2",
                                 Id = "3",
                                 Title = "Item 2"
                             }
                         }
                        );


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
