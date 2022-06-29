
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
        private TabControl currentTab;

        /// <summary>
        /// The tab initial.
        /// </summary>
        private TabCollection tabCollection;

        private UserControl activePanel;
        
        private ITabPanelContent activeTabPanelContent;

        private static string commandStatus;

        private static ElementFocusData elementFocusData;

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
            var fileManger = globalCommandService.GetCommandManager();
            fileManger.CreateSingletonCommand(nameof(DisplayContent), null, DisplayContent);
            fileManger.CreateSingletonCommand(nameof(DisplayViewerContent), null, DisplayViewerContent);

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
                if (this.RegionContentName == null) return;
                var appConfig = this.appConfigService.GetSession();
                var tabs = appConfig.Find("Tabs").ToList();
                var currentPanel = this.GetCurrentRegion().Name;
                var region = this.regionManager.Regions.Single(r => r.Name.Contains(currentPanel));
                var tabsResult = tabs.Single(tab => tab.ParentInfo.GetAttributeValueByName("Name") == this.RegionContentName);
                tabsResult.RemoveAll();

                foreach (UserControl view in region.Views)
                {
                    if (view is FrameworkElement element)
                    {
                        if (element.DataContext is ITabPanelContent panelContent)
                        {
                            tabsResult.Add(
                            elementRecord =>
                            {
                                elementRecord.Tag = "Tab";
                                elementRecord.Attributes.Add("Id", "{" + panelContent.GetPanelToken() + "}");
                                elementRecord.Attributes.Add("Path", panelContent.GetCurrentPath());
                                elementRecord.Attributes.Add("ViewType", panelContent.GetType().Name);
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
                        if (obj is not TabPanel tabPanel) return;
                        commandStatus = "Add";

                        var token = Guid.NewGuid();
                        var panelView = new SplitPanelView();
                        string path = null;

                        if (this.activePanel is { DataContext: IDirectoryPanel panel })
                            path = panel.GetCurrentPath();
                            
                        if (panelView.DataContext is IDirectoryPanel directoryPanel)
                        {
                            directoryPanel.InitializedViewModel(ref token, path ?? this.activeTabPanelContent.GetCurrentPath());
                            this.currentTab = this.CreateTabControl(token, this.TabContentFormat(directoryPanel.GetCurrentPath()), panelView);
                            tabPanel.Collection.Add(this.currentTab);
                        }
                        this.navigationCommand = (NavigationInvoker)this.commandManager.GetCommand(token);
                        this.navigationCommand.OnExecuteChanged += this.OnExecuteChanged;
                        this.regionManager.AddToRegion(this.GetCurrentRegion().Name, panelView);
                        this.ActivateFilePanel(token);
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

        private static bool viewIsRead;
        
        private static UserControl viewerView;

        public static void DisplayViewerContent(object obj)
        {
            commandStatus = "Add";
            var token = Guid.NewGuid();

            if (elementFocusData.TabPanel is TabPanelViewModel tabPanel)
            {
                if (viewIsRead)
                {
                    if (viewerView != null)
                    {
                        tabPanel.TabCollection.Remove(tabPanel.currentTab);
                        var currentPanel = tabPanel.GetCurrentRegion().Name;
                        var region = tabPanel.regionManager.Regions[currentPanel];           
                        region.Remove(viewerView);
                    }
                }

                viewerView = new ViewerView();
                var panelContent = viewerView.DataContext as ITabPanelContent;
                var vpanelContent = viewerView.DataContext as IViewerPanel;
                vpanelContent.SetViewerContent(obj);
                FindDirectoryPanel(viewerView).InitializedViewModel(ref token, panelContent?.GetCurrentFilePath());

                if (!viewIsRead)
                {
                    tabPanel.currentTab = tabPanel.CreateTabControl(token, "Viewer", viewerView, TabTypes.SettingsViewer);
                    tabPanel.TabCollection.Add(tabPanel.currentTab);
                    tabPanel.regionManager.AddToRegion(tabPanel.GetCurrentRegion().Name, viewerView);
                    tabPanel.ActivateFilePanel(token);
                }
                else
                {
                    tabPanel.currentTab = tabPanel.CreateTabControl(token, "Viewer", viewerView, TabTypes.SettingsViewer);
                    tabPanel.TabCollection.Add(tabPanel.currentTab);
                    tabPanel.regionManager.AddToRegion(tabPanel.GetCurrentRegion().Name, viewerView);
                    tabPanel.ActivateFilePanel(token);
                }

                viewIsRead = true;
            }
        }

        public static void DisplayContent(object obj)
        {
            commandStatus = "Add";
            var token = Guid.NewGuid();
            var directoryPanel = new ViewerView();
            
            if (elementFocusData.TabPanel is TabPanelViewModel tabPanel)
            {
                var panelContent = tabPanel.activeTabPanelContent ?? tabPanel.activePanel.DataContext as ITabPanelContent;
                var vpanelContent = tabPanel.activePanel.DataContext as IViewerPanel;
                vpanelContent.SetViewerContent(obj);

                FindDirectoryPanel(directoryPanel).InitializedViewModel(ref token, panelContent?.GetCurrentFilePath());
                tabPanel.currentTab = tabPanel.CreateTabControl(token, tabPanel.TabContentFormat(panelContent?.GetCurrentFilePath()), directoryPanel);
                tabPanel.TabCollection.Add(tabPanel.currentTab);
                tabPanel.regionManager.AddToRegion(tabPanel.GetCurrentRegion().Name, directoryPanel);
                tabPanel.ActivateFilePanel(token);
            }
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
                this.activeTabPanelContent ??= view.DataContext as ITabPanelContent;

                if (view?.DataContext is ITabPanelContent directoryPanel)
                {
                    directoryPanel.InitializedViewModel(ref token, config.Path);
                    this.commandManager.GetCommand(token).OnExecuteChanged += this.OnExecuteChanged;
                }

                this.regionManager.AddToRegion(this.GetCurrentRegion().Name, view);
                collection.Add(this.CreateTabControl(token, this.TabContentFormat(config.Path), view));
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
        private TabControl CreateTabControl(Guid token, object content, FrameworkElement element, TabTypes tabType = TabTypes.Explorer)
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
                TabType = tabType
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
        /// <param name="obj">
        /// The command.
        /// </param>
        private void OnExecuteChanged(object obj)
        {
            if (obj is ConcreteCommand { Receiver: Navigator navigator })
            {
                this.currentPath = navigator.Path;

                if (this.currentTab != null)
                {
                    this.currentTab.Content = this.TabContentFormat(this.currentPath);
                }
            }
            
            if (obj is ITabPanelContent panelContent)
            {
                this.activeTabPanelContent = panelContent;
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
            this.activePanel = (UserControl)regions.Views.Single(
                view => FindDirectoryPanel((FrameworkElement)view).GetPanelToken() == token);
            regions.Activate(this.activePanel);
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
        }
    }
}
