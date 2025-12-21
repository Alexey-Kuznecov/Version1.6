

namespace UnityCommander.Modules.ToolBar.ViewModels
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Shapes;

    using AlexeyKuznecov.Library.Mvvm.Base;

    using MaterialDesignThemes.Wpf;

    using Prism.Commands;
    using Prism.Mvvm;
    using Prism.Services.Dialogs;
    using UnityCommander.Controls.Ribbon;
    using UnityCommander.Controls.Ribbon.Control;
    using UnityCommander.Core.Mvvm;
    using UnityCommander.Integration.Plugins;
    using UnityCommander.Services.Interfaces;
    using UnityCommander.Services.Interfaces.Settings;

    /// <summary>
    /// The view a view model.
    /// </summary>
    public class ToolBarViewModel : BindableBase, IRibbonManager
    {
        #region Dependency Injection Fields

        private readonly IDialogService dialogService;
        private readonly IPluginLoaderService pluginLoader;
        private readonly IIconProviderService iconProvider;
        private readonly ISettingsProviderService configService;

        #endregion
        
        private ICommand minimizeCommand;
        private Path icon;
        private UserControl userControls;
        private DelegateCommand showDialogCommand;
        private string message;

        public ToolBarViewModel(
            IDialogService dialogService,
            IIconProviderService iconProvider,
            IPluginLoaderService pluginLoaderService,
            IGlobalCommandService globalCommandService,
            ISettingsProviderService configService)
         { 
             this.configService = configService;
            this.pluginLoader = pluginLoaderService;
            this.dialogService = dialogService;
            this.iconProvider = iconProvider;
            this.Message = "This Toolbar View";
            this.Icon = iconProvider.GetIcon("Tag").GetIconPath();
        }

        #region Properties
        public Path Icon
        {
            get => this.icon;
            set => this.SetProperty(ref this.icon, value);
        }

        public UserControl UserControls
        {
            get => this.userControls;
            set => this.SetProperty(ref this.userControls, value);
        }

        public string Message
        {
            get => this.message;
            set => this.SetProperty(ref this.message, value);
        }

        public DelegateCommand ShowDialogCommand =>
            this.showDialogCommand ?? new DelegateCommand(this.ExecuteShowDialogCommand);

        #endregion

        public ICommand MinimizeCommand =>
            new RelayCommand(
                obj =>
                    {
                        this.minimizeCommand.Execute(null);
                    });

        void IRibbonManager.Collapse(ICommand command)
        {
            this.minimizeCommand = command;
        }

        void IRibbonManager.Initial(Ribbon ribbon)
        {
            ribbon.Children.Add(RibbonBuilderExtension.SetSection(this.RibbonFileSectionBuilder())
                .SetSection(this.RibbonViewSectionBuilder())
                .SetSection(this.RibbonLaunchSectionBuilder())
                .BuildGrid());
        }

        public void Configure(RibbonConfig config)
        {
            var appConfig = configService.GetAppConfig();
            config.Visibility = appConfig.RibbonVisibility;
        }

        #region Ribbon File Section

        private RibbonBuilder RibbonFileSectionBuilder()
        {
            return RibbonBuilderExtension.Initial("File").SetGroup(
                "File Operation",
                builder =>
                    {
                        builder.AddButton("Home", this.iconProvider.GetIcon(PackIconKind.Home), /*(RibbonCommand)this.globalCommandManager.GetCommand("DisplayContent")*/ null);
                        builder.AddButton("Folder Shared", this.iconProvider.GetIcon(PackIconKind.FolderShared), null);
                        builder.AddButton("Facebook", this.iconProvider.GetIcon(PackIconKind.Facebook), null);
                        builder.AddButton("Access alarms", this.iconProvider.GetIcon(PackIconKind.AccessAlarms), null);
                        builder.AddList(
                            list =>
                                {
                                    list.AddItem(new RibbonListBoxItem("Box cutter", this.iconProvider.GetIcon(PackIconKind.BoxCutter), null));
                                    list.AddComboBoxItem(
                                        box =>
                                            {
                                                box.AddItem("1 Account Plus", this.iconProvider.GetIcon(PackIconKind.AccountPlus), null);
                                                box.AddItem("Zend", this.iconProvider.GetIcon(PackIconKind.Zend), null);
                                                box.AddItem("Ghost", this.iconProvider.GetIcon(PackIconKind.Ghost), null);
                                                box.AddItem("Tab Plus", this.iconProvider.GetIcon(PackIconKind.TabPlus), null);
                                                box.AddItem("Bug", this.iconProvider.GetIcon(PackIconKind.Bug), null);
                                            });
                                    list.AddDropItem("Выделить", this.iconProvider.GetIcon(PackIconKind.Selection),
                                        box =>
                                            {
                                                box.AddItem("Gamepad", this.iconProvider.GetIcon(PackIconKind.Gamepad), null /*this.globalCommandManager.GetCommand("DisplayContent")*/);
                                                box.AddItem("Funnel", this.iconProvider.GetIcon(PackIconKind.Funnel), null);
                                                box.AddItem("Vkontakte", this.iconProvider.GetIcon(PackIconKind.Vkontakte), null);
                                                box.AddItem("About Circle", this.iconProvider.GetIcon(PackIconKind.AboutCircle), null);
                                            });
                                });
                        builder.AddList(
                            list =>
                                {
                                    list.AddItem(new RibbonListBoxItem("Abc", this.iconProvider.GetIcon(PackIconKind.Abc), null));
                                    list.AddComboBoxItem(
                                        box =>
                                            {
                                                box.AddItem("2 Account Plus", this.iconProvider.GetIcon(PackIconKind.AccountPlus), null);
                                                box.AddItem("Zend", this.iconProvider.GetIcon(PackIconKind.Zend), null);
                                                box.AddItem("Ghost", this.iconProvider.GetIcon(PackIconKind.Ghost), null);
                                                box.AddItem("Tab Plus", this.iconProvider.GetIcon(PackIconKind.TabPlus), null);
                                                box.AddItem("Bug", this.iconProvider.GetIcon(PackIconKind.Bug), null);
                                            });
                                    list.AddItem(new RibbonListBoxItem("Facebook", this.iconProvider.GetIcon(PackIconKind.Facebook), null));
                                });
                    }).SetGroup(
                "View Group",
                builder =>
                    {
                        builder.AddButton("Comment", this.iconProvider.GetIcon("Comment"), null);
                        builder.AddButton("Table", this.iconProvider.GetIcon("TableColumn"), null);
                    }).Build();
        }

        #endregion

        #region Ribbon View Section

        private RibbonBuilder RibbonLaunchSectionBuilder()
        {
            return RibbonBuilderExtension.Initial("Launch").SetGroup(
                    "Game Launch",
                    builder =>
                        {
                            builder.AddButton("Comment", this.iconProvider.GetIcon("Comment"), null);
                            builder.AddButton("Settings", this.iconProvider.GetIcon("Settings"), null);
                            builder.AddButton("File tree", this.iconProvider.GetIcon("FileTree"), null);
                        }).Build();
        }

        #endregion

        #region Ribbon View Section

        private RibbonBuilder RibbonViewSectionBuilder()
        {
            return RibbonBuilderExtension.Initial("View").SetGroup(
                    "View Group",
                builder =>
                        {
                            builder.AddButton("Comment", this.iconProvider.GetIcon("Comment"), null);
                            builder.AddButton("File Tree", this.iconProvider.GetIcon("FileTree"), null);
                            builder.AddButton("Tag", this.iconProvider.GetIcon("Tag"), null);
                            builder.AddButton("FileTree", this.iconProvider.GetIcon("FileTree"), null);
                        }).SetGroup(
                    "Shared Group",
                    builder =>
                        {
                            builder.AddButton("Plugin", this.iconProvider.GetIcon("Plugin"), null);
                            builder.AddButton("Table column", this.iconProvider.GetIcon("TableColumn"), null);
                            builder.AddButton("Tag", this.iconProvider.GetIcon("Tag"), null);
                        }).Build();
        }

        #endregion

        private void ExecuteShowDialogCommand()
        {
            var dialogs = this.pluginLoader.GetDialogService();

            foreach (var dialog in dialogs)
            {
                this.dialogService.ShowDialog("DialogPlugin", new OverrideDialogParameters(dialog), r => { });
            }
        }
    }
}
