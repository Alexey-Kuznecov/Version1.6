
namespace UnityCommander.Modules.TabPanel.ViewModels
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Media;
    using Prism.Commands;
    using Prism.Mvvm;
    using Prism.Regions;
    using Common.Models.Icons;

    using Core.Commands;
    using Core.Commands.Base;
    using Modules.FilePanel.Views;
    using Modules.Viewer.Views;
    using Services.Interfaces;
    using Services.Interfaces.Database.Queries.Xml;
    using TabControl = UnityCommander.Controls.TabPanel.TabControl;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;

    using UnityCommander.Common.Module;
    using UnityCommander.Controls.TabPanel;
    using UnityCommander.Modules.FilePanel.ViewModels;
    using UnityCommander.Modules.TabPanel.Behaviors;

    public class TabPanelViewModel : BindableBase, ITabPanel, IElementFocusable
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
        /// The token display.
        /// </summary>
        private string currentPath;

        /// <summary>
        /// The token display.
        /// </summary>
        private static TabPanelViewModel currentTabPanel;

        /// <summary>
        /// The token display.
        /// </summary>
        private ITabPanelContent currentTabPanelContent;

        /// <summary>
        /// The token display.
        /// </summary>
        private TabControl currentTab;

        /// <summary>
        /// The tab initial.
        /// </summary>
        private TabCollection tabCollection;
        private static string commandStatus;

        private static ElementFocusData elementFocusData;

        private static readonly ObservableCollection<TabPanelManager> tabPanelManager = new ();

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="TabPanelViewModel"/> class.
        /// </summary>
        /// <param name="regionManager">
        /// The region commandManager.
        /// </param>
        /// <param name="commandService">
        /// The service that respond for composite commands.
        /// </param>
        /// <param name="configService">
        /// The config Service.
        /// </param>
        /// <param name="globalCommandService">
        /// Global command service
        /// </param>
        /// <param name="manager">
        /// The commandManager.
        /// </param>
        /// <remarks>
        /// TODO: Optimization task, the constructor of this class is called 3 times, although it must be called 2 times. Since the module has only 2 panels.  
        /// </remarks>
        public TabPanelViewModel(
            IRegionManager regionManager,
            IMultiCommandService commandService,
            IAppConfigService configService,
            IGlobalCommandService globalCommandService,
            CommandManager manager)
        {
            tabPanelManager.Add(new TabPanelManager()
            {
                TabCollection = TabCollection,
                TabPanel = this
            });

            var fileManger = globalCommandService.GetCommandManager();
            fileManger.CreateSingletonCommand(nameof(DisplayContent), tabPanelManager, DisplayContent);

            this.regionManager = regionManager;
            this.commandManager = manager;
            this.appConfigService = configService;

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

        public string RegionContentName { get; set; }

        #endregion

        #region Commands

        /// <summary>
        /// The command serializes the state of the file panel after the program is closed
        /// to restore it to its original state the next time it starts.
        /// </summary>
        public DelegateCommand SavePanelStateCommand => new DelegateCommand(
            () =>
            {
                return;

                if (this.RegionContentName == null) return;
                var appConfig = this.appConfigService.GetSession();
                var tabs = appConfig.Find("Tabs").ToList();
                var currentPanel = this.GetCurrentRegion().Name;
                var region = this.regionManager.Regions.Single(r => r.Name.Contains(currentPanel));
                var tabsResult = tabs.Single(tab => tab.ParentInfo.GetAttributeValueByName("Name") == this.RegionContentName);
                tabsResult.RemoveAll();

                foreach (var view in region.Views)
                {
                    if (view is FrameworkElement element)
                    {
                        var directoryPanel = (ITabPanelContent)element.DataContext;
                        if (directoryPanel is IDirectoryPanel panelContent)
                        {
                            tabsResult.Add(
                            elementRecord =>
                            {
                                elementRecord.Tag = "Tab";
                                elementRecord.Attributes.Add("Id", "{" + directoryPanel.GetPanelToken() + "}");
                                elementRecord.Attributes.Add("Path", panelContent.GetCurrentPath());
                                return elementRecord;
                            });
                        }
                    }
                }

                appConfig.Save();
            });

        
        /// <summary>
        ///  Goes back to the previous directory.
        /// </summary>
        public DelegateCommand<object> AddNewTabCommand =>
            new DelegateCommand<object>(
                obj =>
                    {
                        if (obj is TabPanel tabPanel)
                        {
                            commandStatus = "Add";

                            var token = Guid.NewGuid();
                            var panelView = new SplitPanelView();

                            if (panelView.DataContext is IDirectoryPanel directoryPanel)
                            {
                                directoryPanel.InitializedViewModel(ref token, null);
                                this.currentTab = this.CreateTabControl(token, this.TabContentFormat(directoryPanel.GetCurrentPath()), panelView);
                                tabPanel.Collection.Add(this.currentTab);
                            }

                            this.regionManager.AddToRegion(this.GetCurrentRegion().Name, panelView);
                            this.navigationCommand = (NavigationInvoker)this.commandManager.GetCommand(token);
                            this.navigationCommand.OnExecuteChanged += this.OnExecuteChanged;
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
                    this.ActivateFilePanel((Guid)token);
                });

        /// <summary>
        /// The close tab command.
        /// </summary>
        public DelegateCommand<object[]> CloseTabCommand =>
            new DelegateCommand<object[]>(
                view =>
                {
                    commandStatus = "Close";

                    var currentPanel = this.GetCurrentRegion().Name;
                    var region = this.regionManager.Regions[currentPanel];
                    this.TabCollection.Remove((TabControl)view[0]);
                    region.Remove(view[2]);
                });
        
        #endregion

        public static void DisplayContent(object obj)
        {
            var token = Guid.NewGuid();
            var directoryPanel = new ViewerView();
            currentTabPanel.currentTab = currentTabPanel.CreateTabControl(token, elementFocusData.TabContent.GetCurrentPath(), directoryPanel);
            currentTabPanel.TabCollection.Add(currentTabPanel.currentTab);

            FindDirectoryPanel(directoryPanel).InitializedViewModel(ref token, currentTabPanel.currentPath);
            currentTabPanel.regionManager.AddToRegion(currentTabPanel.GetCurrentRegion().Name, directoryPanel);
            currentTabPanel.ActivateFilePanel(token);
        }

        /// <summary>
        /// The initial panel.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        public void InitialTabPanelContent(string name)
        {
            this.RegionContentName = name;
            var collection = new TabCollection();

            foreach (var config in this.appConfigService.GetSession().GetTabConfigs(this.RegionContentName))
            {
                var view = config.ViewType;
                var token = config.Token;

                if (view.DataContext is ITabPanelContent directoryPanel)
                {
                    directoryPanel.InitializedViewModel(ref token, config.Path);
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
        private TabControl CreateTabControl(Guid token, object content, FrameworkElement element)
        {
            TabControl button = new TabControl
            {
                Content = content,
                Margin = new Thickness(0, 0, 1, 0),
                Background = new SolidColorBrush(Color.FromRgb(224, 229, 241)),
                Foreground = new SolidColorBrush(Color.FromRgb(47, 78, 79)),
                BorderBrush = null,
                CloseCommand = this.CloseTabCommand,
                Command = this.ActivateTabCommand,
                CommandParameter = token,
                //Tag = content,
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

            if (commandStatus == "Close") return;

            if (arg == null) return;
            foreach (Control tab in arg.Collection)
            {
                if (tab is AddTabControl) continue;
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
                    this.currentTab.Content = TabContentFormat(this.currentPath);
                    this.currentTab.Tag = this.currentPath;
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
            => $"{path[..path.IndexOf(':')]}:{path[(path.LastIndexOf('\\') + 1)..]}".ToUpper();

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
                view => FindDirectoryPanel((FrameworkElement)view).GetPanelToken() == token));
        }

        /// <summary>
        /// The define current region.
        /// </summary>
        /// <returns>
        /// The <see cref="IRegion"/>.
        /// </returns>
        private IRegion GetCurrentRegion() =>
            this.RegionContentName == NestedRegionNames.LeftFilePanelRegion
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
        private static ITabPanelContent FindDirectoryPanel(FrameworkElement element) =>
            element.DataContext is ITabPanelContent directoryPanel ? directoryPanel : null;

        public void FocusElementDataProvider(ElementFocusData focusData)
        {
            elementFocusData = focusData;
            currentTabPanel = (TabPanelViewModel)focusData.TabPanel;
            this.currentTabPanelContent = focusData.TabContent;
        }
    }
}
