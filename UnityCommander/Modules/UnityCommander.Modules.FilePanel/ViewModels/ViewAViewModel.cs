
namespace UnityCommander.Modules.FilePanel.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Media;

    using Prism.Commands;
    using Prism.Regions;

    using UnityCommander.Common.Models.Icons;
    using UnityCommander.Controls.Taber;
    using UnityCommander.Core.Commands;
    using UnityCommander.Core.Commands.Base;
    using UnityCommander.Core.Modules;
    using UnityCommander.Core.Mvvm;
    using UnityCommander.Modules.FilePanel.Views;
    using UnityCommander.Services.Interfaces;

    using CommandManager = UnityCommander.Core.Commands.CommandManager;

    /// <summary>
    /// The view a view model.
    /// </summary>
    [SuppressMessage("ReSharper", "StyleCop.SA1503")]
    public class ViewAViewModel : RegionViewModelBase, IPanelContainer
    {
        #region Private Fields

        /// <summary>
        /// The command manager.
        /// </summary>
        private readonly CommandManager commandManager;

        /// <summary>
        /// The navigationCommand class instance.
        /// </summary>
        private NavigationInvoker navigationCommand;

        /// <summary>
        /// The navigationCommand class instance.
        /// </summary>
        private NavigationInvoker navigationRCommand;

        /// <summary>
        /// The region manager.
        /// </summary>
        private IRegionManager regionManager;

        /// <summary>
        /// The computer icon.
        /// </summary>
        private IIcon thisComputerIcon;

        /// <summary>
        /// The computer icon.
        /// </summary>
        private bool thisComputerIconIsEnabled;

        /// <summary>
        /// The computer icon.
        /// </summary>
        private bool backButtonIsEnabled;

        /// <summary>
        /// The token display.
        /// </summary>
        private string displayToken;

        /// <summary>
        /// The tab initial.
        /// </summary>
        private TabCollection tabCollection;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewAViewModel"/> class.
        /// </summary>
        /// <param name="regionManager">
        /// The region commandManager.
        /// </param>
        /// <param name="iconProvider">
        /// The icon Provider.
        /// </param>
        /// <param name="manager">
        /// The commandManager.
        /// </param>
        public ViewAViewModel(IRegionManager regionManager, IIconProviderService iconProvider, CommandManager manager)
            : base(regionManager)
        {
            this.regionManager = regionManager;
            this.commandManager = manager;
            this.ThisComputerIcon = iconProvider.GetIcon(MaterialDesignThemes.Wpf.PackIconKind.LaptopWindows);
            this.ThisComputerIconIsEnabled = true;
            this.BackButtonIsEnabled = true;
        }

        #region Public Properties

         /// <summary>
        /// Gets or sets a computer icon.
        /// </summary>
        public TabCollection TabCollection
        {
            get => this.tabCollection;
            set => this.SetProperty(ref this.tabCollection, value);
        }

        /// <summary>
        /// Gets or sets a computer icon.
        /// </summary>
        public IIcon ThisComputerIcon
        {
            get => this.thisComputerIcon;
            set => this.SetProperty(ref this.thisComputerIcon, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether this computer icon is enabled.
        /// </summary>
        public bool ThisComputerIconIsEnabled
        {
            get => this.thisComputerIconIsEnabled;
            set => this.SetProperty(ref this.thisComputerIconIsEnabled, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether back button is enabled.
        /// </summary>
        public bool BackButtonIsEnabled
        {
            get => this.backButtonIsEnabled;
            set => this.SetProperty(ref this.backButtonIsEnabled, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether back button is enabled.
        /// </summary>
        public string DisplayToken
        {
            get => this.displayToken;
            set => this.SetProperty(ref this.displayToken, value);
        }

        #endregion

        #region Commands

        /// <summary>
        ///  Goes back to the previous directory.
        /// </summary>
        public DelegateCommand<object> GoBackDirectoryLeftPanelCommand =>
            new DelegateCommand<object>(
                index =>
        {
            if (this.navigationCommand.CanUndo)
                this.navigationCommand.Previous();

            if (!this.navigationCommand.CanUndo)
            {
                this.navigationCommand.OnExecuteChanged += OnExecuteChanged;
                this.BackButtonIsEnabled = false;
                this.ThisComputerIconIsEnabled = false;
            }
        });

        /// <summary>
        ///  Goes to the device and drive panel.
        /// </summary>
        public DelegateCommand<object> GoDrivePanelCommand => new DelegateCommand<object>(
            index =>
                {
                    var command = (ConcreteCommand)this.navigationCommand.FirstCommand;

                    if (command?.Receiver is Navigator navigator)
                    {
                        if (navigator.Path.Contains("Root:"))
                        {
                            navigator.CommandArg.Invoke(navigator.Path);
                        }
                    }

                    this.navigationCommand.OnExecuteChanged += OnExecuteChanged;
                    this.ThisComputerIconIsEnabled = false;
                });

        /// <summary>
        ///  Goes back to the previous directory.
        /// </summary>
        public DelegateCommand<object> GoBackDirectoryRightPanelCommand =>
            new DelegateCommand<object>(
                index =>
                    {
                        if (this.navigationRCommand.CanUndo)
                            this.navigationRCommand.Previous();

                        if (!this.navigationRCommand.CanUndo)
                        {
                            this.navigationRCommand.OnExecuteChanged += OnExecuteChanged;
                            this.BackButtonIsEnabled = false;
                            this.ThisComputerIconIsEnabled = false;
                        }
                    });

        /// <summary>
        ///  Goes back to the previous directory.
        /// </summary>
        public DelegateCommand<object> AddNewTabCommand =>
            new DelegateCommand<object>(
                obj =>
                    {
                        var newDirectoryPanel = new SplitPanelView();
                        var directoryPanel = this.FindDirectoryPanel(newDirectoryPanel);
                        directoryPanel.InitializedViewModel();
                        var token = directoryPanel.GetPanelToken();
                        this.regionManager.AddToRegion(NestedRegionNames.LeftPanelRegion, newDirectoryPanel);

                        if (obj is TaberPanel control)
                        {
                            control.InitialElements.Add(this.CreateTabControl(token, $"Tab Control {control.InitialElements.Count}"));
                        }

                        this.DisplayToken = token.ToString();
                    });
        
        /// <summary>
        ///  Goes back to the previous directory.
        /// </summary>
        public DelegateCommand<object> ActivateTabCommand =>
            new DelegateCommand<object>(
                number =>
                    {
                        this.regionManager.Regions[NestedRegionNames.LeftPanelRegion]
                             .Activate(this.regionManager.Regions[NestedRegionNames.LeftPanelRegion].Views
                                 .Single(view => FindDirectoryPanel((FrameworkElement)view).GetPanelToken() == (Guid)number));
                    });

        #endregion

        /// <summary>
        /// The initial panel.
        /// </summary>
        /// <param name="directoryPanel">
        /// The directory Panel.
        /// </param>
        public void InitialDirectoryPanel(IDirectoryPanel directoryPanel)
        {
            if (directoryPanel.GetInstanceNumber() == 2)
            {
                var token = directoryPanel.GetPanelToken();

                var control = new TabCollection
                {
                    this.CreateTabControl(token, "Tab Control"),
                    this.CreateAddTabControl()
                };
                this.TabCollection = control;
                this.navigationCommand = (NavigationInvoker)this.commandManager.GetCommand(token);
            }

            if (directoryPanel.GetInstanceNumber() == 1)
            {
                var token = directoryPanel.GetPanelToken();

                this.navigationRCommand = (NavigationInvoker)this.commandManager.GetCommand(token);
            } 
        }

        /// <summary>
        /// The create control tab.
        /// </summary>
        /// <param name="token">
        /// The token.
        /// </param>
        /// <param name="content">
        /// The content.
        /// </param>
        /// <returns>
        /// The <see cref="Button"/>.
        /// </returns>
        private TaberControl CreateTabControl(Guid token, string content)
        {
            TaberControl button = new TaberControl
            {
                Content = content,
                Margin = new Thickness(0, 0, 1, 0),
                Background = new SolidColorBrush(Color.FromRgb(224, 229, 241)),
                Foreground = new SolidColorBrush(Color.FromRgb(47, 78, 79)),
                BorderBrush = null,
                Command = this.ActivateTabCommand,
                CommandParameter = token
            };

            return button;
        }

        /// <summary>
        /// The create control tab.
        /// </summary>
        /// <returns>
        /// The <see cref="Button"/>.
        /// </returns>
        private TaberControl CreateAddTabControl()
        {
            Binding binding = new Binding
            {
                RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(Panel), 1)
            };

            TaberControl button = new TaberControl
            {
                Content = "+",
                Margin = new Thickness(0, 0, 1, 0),
                Background = null,
                Foreground = new SolidColorBrush(Color.FromRgb(47, 78, 79)),
                BorderBrush = null,
                Command = this.AddNewTabCommand
            };

            button.SetBinding(TaberControl.CommandParameterProperty, binding);
            return button;
        }

        /// <summary>
        /// The find directory panel token.
        /// </summary>
        /// <param name="element">
        /// The collection.
        /// </param>
        /// <returns>
        /// The <see cref="Guid"/>.
        /// </returns>
        private IDirectoryPanel FindDirectoryPanel(FrameworkElement element) =>
            element.DataContext is IDirectoryPanel directoryPanel ? directoryPanel : null;

        /// <summary>
        /// Goes back to the previous directory.
        /// </summary>
        /// <param name="command">
        /// The command.
        /// </param>
        private void OnExecuteChanged(ConcreteCommand command)
        {
            if (command.Receiver is Navigator navigator)
            {
                if (!navigator.Path.Contains("Root:"))
                {
                    this.ThisComputerIconIsEnabled = true;
                    this.BackButtonIsEnabled = true;
                }
            }
        }
    }
}