

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
    using UnityCommander.Modules.ToolBar.Views;
    using UnityCommander.Services.Interfaces;

    /// <summary>
    /// The view a view model.
    /// </summary>
    public class ToolBarViewModel : BindableBase, IRibbonManager
    {
        #region Dependency Injection Fields

        /// <summary>
        /// The dialog service.
        /// </summary>
        private readonly IDialogService dialogService;

        /// <summary>
        /// The plugin loader service.
        /// </summary>
        private readonly IPluginLoaderService pluginLoader;

        /// <summary>
        /// The plugin loader service.
        /// </summary>
        private readonly IIconProviderService iconProvider;

        #endregion

        /// <summary>
        /// Gets or sets the minimize command.
        /// </summary>
        private ICommand minimizeCommand;

        /// <summary>
        /// The icon.
        /// </summary>
        private Path icon;

        /// <summary>
        /// The user controls.
        /// </summary>
        private UserControl userControls;

        /// <summary>
        /// The show dialog command.
        /// </summary>
        private DelegateCommand showDialogCommand;

        /// <summary>
        /// The message.
        /// </summary>
        private string message;

        /// <summary>
        /// Initializes a new instance of the <see cref="ToolBarViewModel"/> class.
        /// </summary>
        /// <param name="dialogService">
        /// The dialog Service.
        /// </param>
        /// <param name="iconProvider">
        /// The icon Provider.
        /// </param>
        /// <param name="pluginLoaderService">
        /// The plugin Loader Service.
        /// </param>
        public ToolBarViewModel(IDialogService dialogService, IIconProviderService iconProvider, IPluginLoaderService pluginLoaderService)
        {
            this.pluginLoader = pluginLoaderService;
            this.dialogService = dialogService;
            this.iconProvider = iconProvider;
            this.Message = "This Toolbar View";
            this.UserControls = new MainTabControl();
            this.Icon = iconProvider.GetIcon("Tag").GetIconPath();
        }

        #region Properties

        /// <summary>
        /// Gets or sets the icon.
        /// </summary>
        public Path Icon
        {
            get => this.icon;
            set => this.SetProperty(ref this.icon, value);
        }

        /// <summary>
        /// Gets or sets the user controls.
        /// </summary>
        public UserControl UserControls
        {
            get => this.userControls;
            set => this.SetProperty(ref this.userControls, value);
        }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public string Message
        {
            get => this.message;
            set => this.SetProperty(ref this.message, value);
        }

        /// <summary>
        /// The show dialog command.
        /// </summary>
        public DelegateCommand ShowDialogCommand =>
            this.showDialogCommand ?? new DelegateCommand(this.ExecuteShowDialogCommand);

        #endregion
        
        /// <summary>
        /// Gets or sets the minimize command.
        /// </summary>
        public ICommand MinimizeCommand =>
            new RelayCommand(
                obj =>
                    {
                        this.minimizeCommand.Execute(null);
                    });

        /// <summary>
        /// The maximize.
        /// </summary>
        /// <param name="command">
        /// The command.
        /// </param>
        void IRibbonManager.Collapse(ICommand command)
        {
            this.minimizeCommand = command;
        }

        /// <summary>
        /// The initial.
        /// </summary>
        /// <param name="ribbon">
        /// The ribbon.
        /// </param>
        void IRibbonManager.Initial(Ribbon ribbon)
        {
            ribbon.Children.Add(RibbonBuilderExtension.SetSection(this.RibbonFileSectionBuilder())
                .SetSection(this.RibbonViewSectionBuilder())
                .SetSection(this.RibbonLaunchSectionBuilder())
                .BuildGrid());
        }

        #region Ribbon File Section

        /// <summary>
        /// The ribbon build.
        /// </summary>
        /// <returns>
        /// The <see cref="RibbonBuilder"/>.
        /// </returns>
        private RibbonBuilder RibbonFileSectionBuilder()
        {
            return RibbonBuilderExtension.Initial("File").SetGroup(
                "File Operation",
                builder =>
                    {
                        builder.AddButton("Home", this.iconProvider.GetIcon(PackIconKind.Home), this.ShowDialogCommand);
                        builder.AddButton("Folder Shared", this.iconProvider.GetIcon(PackIconKind.FolderShared), this.ShowDialogCommand);
                        builder.AddButton("Facebook", this.iconProvider.GetIcon(PackIconKind.Facebook), this.ShowDialogCommand);
                        builder.AddButton("Access alarms", this.iconProvider.GetIcon(PackIconKind.AccessAlarms), this.ShowDialogCommand);
                        builder.AddList(
                            list =>
                                {
                                    list.AddItem(new RibbonListBoxItem("Box cutter", this.iconProvider.GetIcon(PackIconKind.BoxCutter), this.ShowDialogCommand));
                                    list.AddComboBoxItem(
                                        box =>
                                            {
                                                box.AddItem("Account Plus", this.iconProvider.GetIcon(PackIconKind.AccountPlus), null);
                                                box.AddItem("Zend", this.iconProvider.GetIcon(PackIconKind.Zend), null);
                                                box.AddItem("Ghost", this.iconProvider.GetIcon(PackIconKind.Ghost), null);
                                                box.AddItem("Tab Plus", this.iconProvider.GetIcon(PackIconKind.TabPlus), null);
                                                box.AddItem("Bug", this.iconProvider.GetIcon(PackIconKind.Bug), null);
                                            });
                                    list.AddComboBoxItem(
                                        box =>
                                            {
                                                box.AddItem("Gamepad", this.iconProvider.GetIcon(PackIconKind.Gamepad), this.ShowDialogCommand);
                                                box.AddItem("Funnel", this.iconProvider.GetIcon(PackIconKind.Funnel), null);
                                                box.AddItem("Vkontakte", this.iconProvider.GetIcon(PackIconKind.Vkontakte), null);
                                                box.AddItem("About Circle", this.iconProvider.GetIcon(PackIconKind.AboutCircle), null);
                                            });
                                });
                        builder.AddList(
                            list =>
                                {
                                    list.AddItem(new RibbonListBoxItem("Abc", this.iconProvider.GetIcon(PackIconKind.Abc), this.ShowDialogCommand));
                                    list.AddComboBoxItem(
                                        box =>
                                            {
                                                box.AddItem("Account Plus", this.iconProvider.GetIcon(PackIconKind.AccountPlus), this.ShowDialogCommand);
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
                        builder.AddButton("Comment", this.iconProvider.GetIcon("Comment"), this.ShowDialogCommand);
                        builder.AddButton("Table", this.iconProvider.GetIcon("TableColumn"), this.ShowDialogCommand);
                    }).Build();
        }

        #endregion

        #region Ribbon View Section

        /// <summary>
        /// The ribbon view build.
        /// </summary>
        /// <returns>
        /// The <see cref="RibbonBuilder"/>.
        /// </returns>
        private RibbonBuilder RibbonLaunchSectionBuilder()
        {
            return RibbonBuilderExtension.Initial("Launch").SetGroup(
                    "Game Launch",
                    builder =>
                        {
                            builder.AddButton("Comment", this.iconProvider.GetIcon("Comment"), this.ShowDialogCommand);
                            builder.AddButton("Settings", this.iconProvider.GetIcon("Settings"), this.ShowDialogCommand);
                            builder.AddButton("File tree", this.iconProvider.GetIcon("FileTree"), this.ShowDialogCommand);
                        }).Build();
        }

        #endregion

        #region Ribbon View Section

        /// <summary>
        /// The ribbon view build.
        /// </summary>
        /// <returns>
        /// The <see cref="RibbonBuilder"/>.
        /// </returns>
        private RibbonBuilder RibbonViewSectionBuilder()
        {
            return RibbonBuilderExtension.Initial("View").SetGroup(
                    "View Group",
                builder =>
                        {
                            builder.AddButton("Comment", this.iconProvider.GetIcon("Comment"), this.ShowDialogCommand);
                            builder.AddButton("File Tree", this.iconProvider.GetIcon("FileTree"), this.ShowDialogCommand);
                            builder.AddButton("Tag", this.iconProvider.GetIcon("Tag"), this.ShowDialogCommand);
                            builder.AddButton("FileTree", this.iconProvider.GetIcon("FileTree"), this.ShowDialogCommand);
                        }).SetGroup(
                    "Shared Group",
                    builder =>
                        {
                            builder.AddButton("Plugin", this.iconProvider.GetIcon("Plugin"), this.ShowDialogCommand);
                            builder.AddButton("Table column", this.iconProvider.GetIcon("TableColumn"), this.ShowDialogCommand);
                            builder.AddButton("Tag", this.iconProvider.GetIcon("Tag"), this.ShowDialogCommand);
                        }).Build();
        }

        #endregion

        /// <summary>
        /// The execute show dialog command.
        /// </summary>
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
