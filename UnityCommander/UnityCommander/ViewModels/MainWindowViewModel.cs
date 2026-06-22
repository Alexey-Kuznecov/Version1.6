// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindowViewModel.cs" company="T">
//  Copyright (p) Alexey Kuznecov. All right reserved.
// </copyright>
// <summary>
//   Defines the MainWindowViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Linq;

namespace UnityCommander.ViewModels
{
    using Prism.Commands;
    using Prism.Dialogs;
    using Prism.Events;
    using Prism.Mvvm;
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;
    using UnityCommander.Common.Commands;
    using UnityCommander.Controls.Window;
    using UnityCommander.Mvvm;
    using UnityCommander.Services;
    using UnityCommander.Services.Interfaces;
    using UnityCommander.Services.Interfaces.Settings;
    using UnityCommander.Services.Layout;
    using UnityCommander.WPF.Behaviors;

    /// <summary>
    /// The main window view model.
    /// </summary>
    public class MainWindowViewModel : BindableBase
    {
        private CommandExecutionService _commandExecutionService;

        private IShellLayoutManager _shellLayoutManager;

        private IMultiCommandService stateCommands;

        private CustomViewModel importCustomWindow;

        private string icon;

        private double _ribbonHeight = 0;

        private double _bottomHeight = 0;

        public MainWindowViewModel(
            IDialogService dialogService,
            IEventAggregator exchange,
            ISettingsProviderService settingsProviderService,
            IMultiCommandService command,
            IShellLayoutManager shellLayoutManager,
            CommandExecutionService commandService)
        {
            _commandExecutionService = commandService;
            _shellLayoutManager = shellLayoutManager;
            _shellLayoutManager.AreaChanged += OnAreaChanged;

            this.StateCommand = command;
            
            this.StateCommand
                .SaveCommand
                .RegisterCommand(this.CloseWindowCommand);

            var settings = settingsProviderService.GetAppConfig();
                      
            this.Icon = Directory.GetCurrentDirectory() + "\\icon.ico";
        }

        public string Icon
        {
            get => this.icon;
            set => this.SetProperty(ref this.icon, value);
        }

        public double RibbonHeight
        {
            get => _ribbonHeight;
            set => SetProperty(ref _ribbonHeight, value);
        }

        public double BottomHeight
        {
            get => _bottomHeight;
            set => SetProperty(ref _bottomHeight, value);
            
        }

        private void OnAreaChanged(object? sender, ShellAreaChangedEventArgs e)
        {
            switch (e.Area)
            {
                case ShellArea.Ribbon:
                    RibbonHeight = e.State.Size;
                    break;

                case ShellArea.BottomPanel:
                    BottomHeight = e.State.Size;
                    break;
            }
        }

        public CustomViewModel ImportCustomWindow
        {
            get => this.importCustomWindow;
            set
            {
                if (value != null)
                {
                    value.CloseCommand = this.StateCommand.SaveCommand;
                    this.SetProperty(ref this.importCustomWindow, value);
                    this.importCustomWindow.CollapseRibbonCommand = new RelayCommand(obj =>
                    {
                        this.importCustomWindow.CollapseContent = (this.importCustomWindow.CollapseContent as string) == "Max" ? "Mix" : "Max";
                        _commandExecutionService.ExecuteAsync(CommandNames.UI.ToggleRibbon);
                    });
                }
            }
        }

        public IMultiCommandService StateCommand
        {
            get => this.stateCommands;
            set => this.SetProperty(ref this.stateCommands, value);
        }

        public DelegateCommand<Window> CloseWindowCommand => new DelegateCommand<Window>(
            window =>
            {
                Application.Current.Shutdown();
            });
    }
}
