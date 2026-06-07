

namespace UnityCommander.Modules.ToolBar.ViewModels
{
    using CommandSystem.Abstractions;
    using CommandSystem.Core.Factory;
    using Prism.Dialogs;
    using Prism.Mvvm;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using System.Windows.Shapes;
    using UnityCommander.Common.Commands;
    using UnityCommander.Common.State;
    using UnityCommander.Modules.ToolBar.Commands;
    using UnityCommander.Ribbon.Core.Models;
    using UnityCommander.Ribbon.Core.Models.Controls;
    using UnityCommander.Ribbon.Core.Services;
    using UnityCommander.Services;
    using UnityCommander.Services.Interfaces;
    using UnityCommander.Services.Interfaces.Settings;
    using UnityCommander.Services.Layout;
    using UnityCommander.Core.Commands;
    using System;

    /// <summary>
    /// The view a view model.
    /// </summary>
    public class ToolBarViewModel : BindableBase
    {
        #region Dependency Injection Fields

        private readonly IDialogService dialogService;
        private readonly IIconProviderService iconProvider;
        private readonly ISettingsProviderService configService;
        private readonly CommandService _commandService;
        private IShellLayoutManager _shellLayoutManager;

        #endregion

        private Path icon;
        private string message;

        public IRibbonManager RibbonManager { get; }

        public ToolBarViewModel(
            IDialogService dialogService,
            IIconProviderService iconProvider,
            ISettingsProviderService configService,
            IRibbonManager ribbonManager,
            IShellLayoutManager shellLayoutManager,
            CommandService commandService)
        {
            _shellLayoutManager = shellLayoutManager;
            _commandService = commandService;
            RibbonManager = ribbonManager;
            this.configService = configService;
            this.dialogService = dialogService;
            this.iconProvider = iconProvider;
            IsExpanded = true;

            var ribbon = new RibbonBuilder()
                .AddTab(
                    new RibbonTabBuilder("tab1", "Главная")
                    .AddGroup(new RibbonGroupBuilder("grp1", "Large")
                    .AddSection(sec => sec
                    .WithLayout(RibbonGroupLayout.Inline)
                        .AddButton("btn1", "Команда 1", new ToggleBottomPanel(_commandService, "btn1"), RibbonItemCategory.FileOpen, "file.add")
                        .AddButton("btn2", "Команда 2", new FileRemoveCommand(_commandService, "btn2"), RibbonItemCategory.FileOpen, "file.delete")
                        .AddButton("btn2", "Команда 3", new UndoCommand(_commandService, "btn3"), RibbonItemCategory.FileOpen, "edit.undo")
                        .AddItem(new RibbonCheckBoxModel()
                        {
                            Id = "chk1",
                            Text = "Флажок 1",
                            Command = new DemoCommands()
                        })
                    ).AddSection(sec => sec
                    .WithLayout(RibbonGroupLayout.Large)
                        .AddButton("btn3", "Команда 3", new DemoCommands(), RibbonItemCategory.FileOpen, "file.add")
                        .AddButton("btn4", "Команда 4", new DemoCommands(), RibbonItemCategory.FileOpen, "edit.delete")
                        .AddButton("btn5", "Команда 5", new DemoCommands(), RibbonItemCategory.FileOpen, "file.add")
                        .AddButton("btn6", "Команда 6", new DemoCommands(), RibbonItemCategory.FileOpen, "file.add")
                        .AddItem(new RibbonCheckBoxModel()
                        {
                            Id = "chk1",
                            Text = "Флажок 1",
                            Command = new DemoCommands()
                        })
                        .AddComboBox(cb => cb
                            .AddItem("1", "A _____", "file.add", new DemoCommands())
                            .AddItem("2", "B _____", "clock", new DemoCommands())
                            .WithSelected("1")
                        )
                    ))
                    .AddGroup(new RibbonGroupBuilder("grp1", "Medium")
                        .AddSection(sec => sec
                        .WithLayout(RibbonGroupLayout.Medium)
                            .AddButton("btn1", "Команда 1", new DemoCommands(), RibbonItemCategory.FileOpen, "file.add")
                            .AddButton("btn2", "Команда 2", new DemoCommands(), RibbonItemCategory.FileOpen, "file.add")
                            .AddButton("btn3", "Команда 3", new DemoCommands(), RibbonItemCategory.FileOpen, "file.add")
                            .AddItem(new RibbonCheckBoxModel()
                            {
                                Id = "chk1",
                                Text = "Флажок 1",
                                Command = new DemoCommands()
                            })
                            .AddItem(new RibbonCheckBoxModel()
                            {
                                Id = "chk2",
                                Text = "Флажок 2",
                                Command = new DemoCommands()
                            })
                            .AddItem(new RibbonCheckBoxModel()
                            {
                                Id = "chk3",
                                Text = "Флажок 3",
                                Command = new DemoCommands()
                            })
                        )
                        .AddSection(sec => sec
                        .WithLayout(RibbonGroupLayout.Small)
                            .AddButton("btn1", "Команда 1", new DemoCommands(), RibbonItemCategory.FileOpen, "file.add")
                            .AddButton("btn2", "Команда 2", new DemoCommands(), RibbonItemCategory.FileOpen, "file.add")
                            .AddButton("btn3", "Команда 3", new DemoCommands(), RibbonItemCategory.FileOpen, "file.add")
                            .AddButton("btn4", "Команда 4", new DemoCommands(), RibbonItemCategory.FileOpen, "file.add")
                            .AddButton("btn5", "Команда 5", new DemoCommands(), RibbonItemCategory.FileOpen, "file.add")
                        )
                    ).AddGroup(
                        new RibbonGroupBuilder("grp2", "Small")
                            .AddSection(sec => sec
                                .WithLayout(RibbonGroupLayout.Small)
                                .AddButton("btn1", "Команда 1", new DemoCommands(), RibbonItemCategory.FileOpen, "file.add")
                                .AddButton("btn2", "Команда 2", new DemoCommands(), RibbonItemCategory.FileOpen, "file.add")
                                .AddButton("btn3", "Команда 3", new DemoCommands(), RibbonItemCategory.FileOpen, "file.add")
                                .AddButton("btn4", "Команда 4", new DemoCommands(), RibbonItemCategory.FileOpen, "file.add")
                                )
                    )
                    .AddGroup(new RibbonGroupBuilder("grp3", "(Medium)")
                        .AddSection(sec => sec
                            .WithLayout(RibbonGroupLayout.Medium)
                            .AddButton("btn1", "Команда 1", new DemoCommands(), RibbonItemCategory.FileOpen, "file.add")
                            .AddButton("btn2", "Команда 2", new DemoCommands(), RibbonItemCategory.FileOpen, "file.add")
                            .AddComboBox(builder =>
                            {
                                builder.AddItem("cmb1", "Вариант 1", "file.add", new DemoCommands());
                                builder.AddItem("cmb2", "Вариант 2", "", new DemoCommands());
                                builder.AddItem("cmb3", "Вариант 3", "edit.delete", new DemoCommands());
                                builder.AddItem("cmb4", "Вариант 4", "", new DemoCommands());
                                builder.WithSelected("cmb2");
                                return builder;
                            })
                            )).Build())
                .AddTab(new RibbonTabBuilder("tab2", "Вид")
                    .AddGroup(new RibbonGroupBuilder("grp1", "Настройки")
                        .AddSection(sec => sec
                            .WithLayout(RibbonGroupLayout.Large)
                            .WithCategory(RibbonItemCategory.Default)
                            .AddButton("btn1", "Команда 1", new DemoCommands(), RibbonItemCategory.FileOpen, "file.add")
                        )
                    )
                    .AddGroup(new RibbonGroupBuilder("grp2", "Настройки 2")
                        .AddSection(sec => sec
                            .WithLayout(RibbonGroupLayout.Medium)
                            .AddComboBox(builder =>
                            {
                                builder.AddItem("cmb1", "Вариант 1", "", new DemoCommands());
                                builder.AddItem("cmb2", "Вариант 2", "", new DemoCommands());
                                builder.AddItem("cmb3", "Вариант 3", "", new DemoCommands());
                                builder.AddItem("cmb4", "Вариант 4", "", new DemoCommands());
                                builder.WithSelected("cmb2");
                                return builder;
                            }).AddItem(new RibbonCheckBoxModel()
                            {
                                Id = "chk1",
                                Text = "Флажок 10",
                                Command = new DemoCommands()
                            })
                        )
                    )
                    .AddGroup(new RibbonGroupBuilder("grp3", "Настройки")
                        .AddSection(sec => sec
                            .WithLayout(RibbonGroupLayout.Small)
                            .AddButton("btn1", "Команда 1", new DemoCommands(), RibbonItemCategory.FileOpen, "file.add")
                            .AddButton("btn2", "Команда 2", new DemoCommands(), RibbonItemCategory.FileOpen, "file.add")
                            .AddButton("btn3", "Команда 3", new DemoCommands(), RibbonItemCategory.FileOpen, "file.add")
                        )
                        .AddSection(sec => sec
                            .WithLayout(RibbonGroupLayout.Large)
                            .AddComboBox(cb => cb
                                .AddItem("1", "A", "", new DemoCommands())
                                .AddItem("2", "B", "", new DemoCommands())
                                .WithSelected("1")
                            )
                        )
                        ).Build())
                .AddTab(new RibbonTabBuilder("tab3", "Справка")
                    .AddGroup(new RibbonGroupBuilder("grp1", "Информация")
                        .AddSection(sec => sec
                            .WithLayout(RibbonGroupLayout.Large)
                            .AddButton("btn4", "О программе", new DemoCommands(), RibbonItemCategory.FileOpen, "file.add")
                        )
                    ).Build()
                ).Build();

            RibbonManager.SetModel(ribbon);

            RibbonManager.TabCollapsed += RibbonManager_TabCollapsed;
            RibbonManager.TabExpanded += RibbonManager_TabExpanded;

            commandService.Register(CommandFactoryExtensions.Create(
                 CommandNames.UI.ToggleRibbon,
                 ToggleRibbon
             ));
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
