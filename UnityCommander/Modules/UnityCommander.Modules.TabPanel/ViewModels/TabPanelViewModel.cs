using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using UnityCommander.Common.Models.Icons;
using UnityCommander.Controls.Taber;
using UnityCommander.Core.Commands;
using UnityCommander.Core.Commands.Base;
using UnityCommander.Core.Modules;
using UnityCommander.Integration.Commands;
using UnityCommander.Modules.FilePanel.Views;
using UnityCommander.Modules.Viewer.ViewModels;
using UnityCommander.Modules.Viewer.Views;
using UnityCommander.Services.Interfaces;
using UnityCommander.Services.Interfaces.Database.Queries.Xml;
using TabControl = UnityCommander.Controls.Taber.TabControl;

namespace UnityCommander.Modules.TabPanel.ViewModels
{
    public class TabPanelViewModel : BindableBase, ITabPanel
    {
        #region Private Fields

        /// <summary>
        /// The command manager.
        /// </summary>
        private readonly CommandManager commandManager;

        /// <summary>
        /// The region manager.
        /// </summary>
        private readonly IRegionManager regionManager;

        /// <summary>
        /// The region manager.
        /// </summary>
        private readonly IAppConfigService appConfigService;

        /// <summary>
        /// The navigationCommand class instance.
        /// </summary>
        private NavigationInvoker navigationCommand;

        /// <summary>
        /// The region manager.
        /// </summary>
        private string currentRegionName;

        /// <summary>
        /// The computer icon.
        /// </summary>
        private IIcon thisComputerIcon;

        /// <summary>
        /// The computer icon.
        /// </summary>
        private IIcon backButtonIcon;

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
        private string currentPath;

        /// <summary>
        /// The token display.
        /// </summary>
        private TabControl currentTab;

        /// <summary>
        /// The tab initial.
        /// </summary>
        private TabCollection tabCollection;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="TabPanelViewModel"/> class.
        /// </summary>
        /// <param name="regionManager">
        /// The region commandManager.
        /// </param>
        /// <param name="iconProvider">
        /// The icon Provider.
        /// </param>
        /// <param name="commandService">
        /// The service that respond for composite commands.
        /// </param>
        /// <param name="configService">
        /// The config Service.
        /// </param>
        /// <param name="manager">
        /// The commandManager.
        /// </param>
        /// <remarks>
        /// TODO: Optimization task, the constructor of this class is called 3 times, although it must be called 2 times. Since the module has only 2 panels.  
        /// </remarks>
        public TabPanelViewModel(
            IRegionManager regionManager,
            IIconProviderService iconProvider,
            IMultiCommandService commandService,
            IAppConfigService configService,
            IGlobalCommandService globalCommandService,
            CommandManager manager)
        {
            var fileManger = globalCommandService.GetCommandManager();
            fileManger.CreateCommand("DisplayContent", this, DisplayContent);

            this.regionManager = regionManager;
            this.commandManager = manager;
            this.appConfigService = configService;
            this.ThisComputerIcon = iconProvider.GetIcon(MaterialDesignThemes.Wpf.PackIconKind.LaptopWindows);
            this.BackButtonIcon = iconProvider.GetIcon(MaterialDesignThemes.Wpf.PackIconKind.ArrowBack);
            this.ThisComputerIconIsEnabled = true;
            this.BackButtonIsEnabled = true;

            // Composite command
            commandService.SaveCommand.RegisterCommand(this.SavePanelStateCommand);
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
        /// Gets or sets a computer icon.
        /// </summary>
        public IIcon BackButtonIcon
        {
            get => this.backButtonIcon;
            set => this.SetProperty(ref this.backButtonIcon, value);
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

        #endregion

        #region Commands

        /// <summary>
        /// The command serializes the state of the file panel after the program is closed
        /// to restore it to its original state the next time it starts.
        /// </summary>
        public DelegateCommand SavePanelStateCommand => new DelegateCommand(
            () =>
            {
                if (this.currentRegionName == null) return;
                var appConfig = this.appConfigService.GetSession();
                var tabs = appConfig.Find("Tabs").ToList();
                var currentPanel = this.GetCurrentRegion().Name;
                var region = regionManager.Regions.Single(r => r.Name.Contains(currentPanel));
                var tabsResult = tabs.Single(tab => tab.ParentInfo.GetAttributeValueByName("Name") == this.currentRegionName);
                tabsResult.RemoveAll();

                foreach (var view in region.Views)
                {
                    if (view is FrameworkElement element)
                    {
                        var directoryPanel = (ITabPanelContent)element.DataContext;

                        tabsResult.Add(
                            elementRecord =>
                            {
                                elementRecord.Tag = "Tab";
                                elementRecord.Attributes.Add("Id", "{" + directoryPanel.GetPanelToken() + "}");
                                elementRecord.Attributes.Add("Path", directoryPanel.GetCurrentPath());
                                return elementRecord;
                            });
                    }
                }

                appConfig.Save();
            });

        /// <summary>
        ///  Goes back to the previous directory.
        /// </summary>
        public DelegateCommand<object> GoBackDirectoryPanelCommand =>
            new DelegateCommand<object>(
                index =>
                {
                    this.currentTab.Content = this.TabContentFormat(this.currentPath);

                    if (this.navigationCommand.CanUndo)
                        this.navigationCommand.Previous();

                    if (!this.navigationCommand.CanUndo)
                    {
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

                this.ThisComputerIconIsEnabled = false;
            });

        /// <summary>
        ///  Goes back to the previous directory.
        /// </summary>
        public DelegateCommand<object> AddNewTabCommand =>
            new DelegateCommand<object>(
                obj =>
                {
                    if (obj is UnityCommander.Controls.Taber.TabPanel tabPanel)
                    {
                        var token = Guid.NewGuid();
                        var path = tabPanel.Collection.GetActive().Tag;
                        var directoryPanel = new SplitPanelView();

                        this.currentTab = this.CreateTabControl(token, (string)path, directoryPanel);
                        tabPanel.Collection.Add(this.currentTab);

                        this.FindDirectoryPanel(directoryPanel).InitializedViewModel(token, (string)path);
                        this.regionManager.AddToRegion(this.GetCurrentRegion().Name, directoryPanel);
                        this.navigationCommand = (NavigationInvoker)this.commandManager.GetCommand(token);
                        this.navigationCommand.OnExecuteChanged += OnExecuteChanged;
                        this.ActivateFilePanel(token);
                    }
                });

        /// <summary>
        ///  Goes back to the previous directory.
        /// </summary>
        public DelegateCommand<object> ActivateTabCommand =>
            new DelegateCommand<object>(
                token =>
                {
                    this.navigationCommand = (NavigationInvoker)this.commandManager.GetCommand((Guid)token);

                    if (this.navigationCommand.CanUndo)
                    {
                        this.BackButtonIsEnabled = true;
                        this.ThisComputerIconIsEnabled = true;
                    }

                    if (!this.navigationCommand.CanUndo)
                    {
                        this.BackButtonIsEnabled = false;
                        this.ThisComputerIconIsEnabled = false;
                    }

                    this.ActivateFilePanel((Guid)token);
                });

        /// <summary>
        /// The close tab command.
        /// </summary>
        public DelegateCommand<object[]> CloseTabCommand =>
            new DelegateCommand<object[]>(
                view =>
                {
                    if (this.TabCollection.Count > 2)
                    {
                        var currentPanel = this.GetCurrentRegion().Name;
                        var region = this.regionManager.Regions[currentPanel];
                        this.TabCollection.Remove((TabControl)view[0]);
                        region.Remove(view[2]);
                    }
                });

        #endregion

        public void DisplayContent(object vm)
        {
            var token = Guid.NewGuid();
            var directoryPanel = new ViewerView();

            this.currentTab = this.CreateTabControl(token, null, directoryPanel);
            this.TabCollection.Add(this.currentTab);

            this.FindDirectoryPanel(directoryPanel).InitializedViewModel(token, null);
            this.regionManager.AddToRegion(this.GetCurrentRegion().Name, directoryPanel);
            this.navigationCommand = (NavigationInvoker)this.commandManager.GetCommand(token);
            this.navigationCommand.OnExecuteChanged += OnExecuteChanged;
            this.ActivateFilePanel(token);
        }

        /// <summary>
        /// The initial panel.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        public void InitialTabPanelContent(string name)
        {
            this.currentRegionName = name;
            var collection = new TabCollection();

            foreach (var config in this.appConfigService.GetSession().GetTabConfigs(this.currentRegionName))
            {
                if (!Directory.Exists(config.Path))
                {
                    config.Path = "C:\\";
                }

                var view = new SplitPanelView();
                var token = config.Token;

                if (view.DataContext is ITabPanelContent directoryPanel)
                {
                    directoryPanel.InitializedViewModel(token, config.Path);
                    this.commandManager.GetCommand(token).OnExecuteChanged += this.OnExecuteChanged;
                }

                this.regionManager.AddToRegion(this.GetCurrentRegion().Name, view);
                collection.Add(this.CreateTabControl(token, config.Path, view));
            }

            collection.Add(this.CreateAddTabControl());
            collection.CollectionChanged += this.OnTabCollectionChanged;

            if (collection[0] is TabControl tabControl)
            {
                this.currentTab = tabControl;
                this.currentTab.IsEnabled = false;
            }

            this.TabCollection = collection;
            this.navigationCommand = (NavigationInvoker)this.commandManager.GetCommand((Guid)this.currentTab.CommandParameter);
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
        /// <param name="element">
        /// The element.
        /// </param>
        /// <returns>
        /// The <see cref="System.Windows.Controls.Button"/>.
        /// </returns>
        private TabControl CreateTabControl(Guid token, string content, FrameworkElement element)
        {
            TabControl button = new TabControl
            {
                Content = content != null ? this.TabContentFormat(content) : "Viewer",
                Margin = new Thickness(0, 0, 1, 0),
                Background = new SolidColorBrush(Color.FromRgb(224, 229, 241)),
                Foreground = new SolidColorBrush(Color.FromRgb(47, 78, 79)),
                BorderBrush = null,
                CloseCommand = this.CloseTabCommand,
                Command = this.ActivateTabCommand,
                CommandParameter = token,
                Tag = content,
            };

            button.CloseCommandParameter = new object[] { button, token, element };
            button.TabClick += this.OnTabControlClick;
            return button;
        }

        /// <summary>
        /// The create control tab.
        /// </summary>
        /// <returns>
        /// The <see cref="System.Windows.Controls.Button"/>.
        /// </returns>
        private AddTabControl CreateAddTabControl()
        {
            var binding = new Binding
            {
                RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(Panel), 1)
            };

            var button = new AddTabControl
            {
                Content = "+",
                Margin = new Thickness(0, 0, 1, 0),
                Background = null,
                Foreground = new SolidColorBrush(Color.FromRgb(47, 78, 79)),
                BorderBrush = null,
                Command = this.AddNewTabCommand
            };

            button.SetBinding(AddTabControl.CommandParameterProperty, binding);
            return button;
        }

        #region Event Handlers

        /// <summary>
        /// The tab collection changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void OnTabCollectionChanged(object sender, EventArgs e)
        {
            var arg = e as CollectionChangedEventArg;
            TabControl tabIndex = arg?.Collection[^1] as TabControl;

            if (arg != null)
                foreach (Control tab in arg.Collection)
                {
                    if (tab is AddTabControl) return;
                    tab.IsEnabled = tab != tabIndex;
                }
        }

        /// <summary>
        /// The on tab control click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void OnTabControlClick(object sender, RoutedEventArgs e)
        {
            var control = (TabControl)sender;
            this.currentTab = control;
        }

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
                this.currentPath = navigator.Path;

                if (this.currentTab != null)
                {
                    this.currentTab.Content = this.TabContentFormat(this.currentPath);
                    this.currentTab.Tag = this.currentPath;
                }

                if (!navigator.Path.Contains("Root:"))
                {
                    this.ThisComputerIconIsEnabled = true;
                    this.BackButtonIsEnabled = true;
                }
            }
        }

        #endregion

        /// <summary>
        /// The tab content format.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string TabContentFormat(string path)
            => $"{path.Substring(0, path.IndexOf(':'))}:{path.Substring(path.LastIndexOf('\\') + 1)}".ToUpper();

        /// <summary>
        /// The activate file panel.
        /// </summary>
        /// <param name="token">
        /// The token.
        /// </param>
        private void ActivateFilePanel(Guid token)
        {
            var regions = this.GetCurrentRegion();
            regions.Activate(regions.Views.Single(
                view => this.FindDirectoryPanel((FrameworkElement)view).GetPanelToken() == token));
        }

        /// <summary>
        /// The define current region.
        /// </summary>
        /// <returns>
        /// The <see cref="IRegion"/>.
        /// </returns>
        private IRegion GetCurrentRegion() =>
            this.currentRegionName == NestedRegionNames.LeftFilePanelRegion
                ? this.regionManager.Regions[NestedRegionNames.LeftPanelContentRegion]
                : this.regionManager.Regions[NestedRegionNames.RightPanelContentRegion];

        /// <summary>
        /// The find directory panel token.
        /// </summary>
        /// <param name="element">
        /// The collection.
        /// </param>
        /// <returns>
        /// The <see cref="Guid"/>.
        /// </returns>
        private ITabPanelContent FindDirectoryPanel(FrameworkElement element) =>
            element.DataContext is ITabPanelContent directoryPanel ? directoryPanel : null;
    }
}
