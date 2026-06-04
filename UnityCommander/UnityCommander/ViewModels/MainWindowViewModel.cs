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
    using AlexeyKuznecov.Library.Mvvm.Base;
    using Prism.Commands;
    using Prism.Events;
    using Prism.Mvvm;
    using Prism.Services.Dialogs;
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using UnityCommander.Common.Commands;
    using UnityCommander.Controls.Window;
    using UnityCommander.Core.Behaviors;
    using UnityCommander.Ribbon.Core.Services;
    using UnityCommander.Services.Bootstrap;
    using UnityCommander.Services.Interfaces;
    using UnityCommander.Services.Interfaces.Settings;

    /// <summary>
    /// The main window view model.
    /// </summary>
    public class MainWindowViewModel : BindableBase, IKeyBinding
    {
        private readonly IRibbonManager _ribbonManager;

        private IMultiCommandService stateCommands;

        private CustomViewModel importCustomWindow;

        private string icon;

        private double ribbonContainerSize;

        private Visibility toolBarVisibility;

        public MainWindowViewModel(
            IDialogService dialogService,
            IEventAggregator exchange,
            ISettingsProviderService settingsProviderService,
            IIconProviderService iconProviderService,
            IMultiCommandService command, IRibbonManager ribbonManager)
        {
            _ribbonManager = ribbonManager;
            this.StateCommand = command;
            
            this.StateCommand
                .SaveCommand
                .RegisterCommand(this.CloseWindowCommand);

            var settings = settingsProviderService.GetAppConfig();
            
            this.IsRibbonExpanded = false;
          
            this.Icon = Directory.GetCurrentDirectory() + "\\icon.ico";
            
            _ribbonManager.TabCollapsed += RibbonManager_TabCollapsed;
            _ribbonManager.TabExpanded += RibbonManager_TabExpanded;
        }

        private void RibbonManager_TabExpanded(object sender, RibbonTabEventArgs e)
        {
            RibbonContainerSize = 180;
        }

        private void RibbonManager_TabCollapsed(object sender, RibbonTabEventArgs e)
        {
            RibbonContainerSize = 38;
        }

        public string Icon
        {
            get => this.icon;
            set => this.SetProperty(ref this.icon, value);
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
                        IsRibbonExpanded = !IsRibbonExpanded;
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

        public double RibbonContainerSize
        {
            get => this.ribbonContainerSize;
            set => this.SetProperty(ref this.ribbonContainerSize, value);
        }

        public Visibility ToolBarVisibility
        {
            get => this.toolBarVisibility;
            set => this.SetProperty(ref this.toolBarVisibility, value);
        }

        private bool _isRibbonExpanded;

        public bool IsRibbonExpanded
        {
            get => _isRibbonExpanded;
            set
            {
                if (!SetProperty(ref _isRibbonExpanded, value))
                    return;

                RibbonContainerSize = value ? 180 : 0;
                ToolBarVisibility = value ? Visibility.Visible : Visibility.Hidden;
            }
        }

        public void SetBinding(object dependencyObject, KeyboardManager manager)
        {
            Grid grid = dependencyObject as Grid;

            //foreach (var globalCommand in this.globalCommandManager.GetCommands().Where(globalCommand => globalCommand?.ShortcutKey != null))
            //{
            //    grid?.InputBindings.Add(new InputBinding(globalCommand.Command, globalCommand.ShortcutKey));
            //}
        }
    }
}
