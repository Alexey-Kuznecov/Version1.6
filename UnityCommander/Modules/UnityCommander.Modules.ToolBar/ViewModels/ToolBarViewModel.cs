

namespace UnityCommander.Modules.ToolBar.ViewModels
{
    using System.Windows.Controls;
    using System.Windows.Shapes;

    using MaterialDesignThemes.Wpf;

    using Prism.Commands;
    using Prism.Mvvm;
    using Prism.Services.Dialogs;

    using UnityCommander.Controls.Ribbon;
    using UnityCommander.Core.Mvvm;
    using UnityCommander.Modules.ToolBar.Views;
    using UnityCommander.Services.Interfaces;

    /// <summary>
    /// The view a view model.
    /// </summary>
    public class ToolBarViewModel : BindableBase
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
        /// The ribbon section builder.
        /// </summary>
        private readonly RibbonBuilder ribbonSectionBuilder;

        /// <summary>
        /// The icon.
        /// </summary>
        private Path icon;

        /// <summary>
        /// The user controls.
        /// </summary>
        private UserControl userControls;

        /// <summary>
        /// The file operation tools.
        /// </summary>
        private Ribbon ribbon;

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
            
            this.ribbonSectionBuilder = this.RibbonFileBuild();
            this.ribbonSectionBuilder = this.RibbonViewBuild();
            this.ribbonSectionBuilder = this.RibbonViewBuild2();
            this.Ribbon = this.ribbonSectionBuilder.Build();
        }

        /// <summary>
        /// Gets or sets the icon.
        /// </summary>
        public Path Icon
        {
            get => this.icon;
            set => this.SetProperty(ref this.icon, value);
        }

        /// <summary>
        /// Gets or sets the file operation tools.
        /// </summary>
        public Ribbon Ribbon
        {
            get => this.ribbon;
            set => this.SetProperty(ref this.ribbon, value);
        }

        /// <summary>
        /// The show dialog command.
        /// </summary>
        public DelegateCommand ShowDialogCommand =>
            this.showDialogCommand ?? new DelegateCommand(this.ExecuteShowDialogCommand);

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
        /// The ribbon build.
        /// </summary>
        /// <returns>
        /// The <see cref="RibbonBuilder"/>.
        /// </returns>
        private RibbonBuilder RibbonFileBuild()
        {
            var editGroup = new RibbonGroupBuilder();
            editGroup.AddGroup("File Operation");
            editGroup.AddButton(this.iconProvider.GetIcon(PackIconKind.Home), this.ShowDialogCommand);
            editGroup.AddButton(this.iconProvider.GetIcon(PackIconKind.DesktopWindows), this.ShowDialogCommand);
            editGroup.AddButton(this.iconProvider.GetIcon(PackIconKind.FolderShared), this.ShowDialogCommand);
            editGroup.AddButton(this.iconProvider.GetIcon(PackIconKind.FileCloud), this.ShowDialogCommand);
            editGroup.AddButton(this.iconProvider.GetIcon(PackIconKind.AccessAlarms), this.ShowDialogCommand);

            var copyGroup = new RibbonGroupBuilder();
            copyGroup.AddGroup("View Group");
            copyGroup.AddButton(this.iconProvider.GetIcon("Comment"), this.ShowDialogCommand);
            copyGroup.AddButton(this.iconProvider.GetIcon("TableColumn"), this.ShowDialogCommand);

            var sectionFile = new RibbonBuilder("File");
            sectionFile.SetSection(editGroup);
            sectionFile.SetSection(copyGroup);

            return sectionFile;
        }

        /// <summary>
        /// The ribbon view build.
        /// </summary>
        /// <returns>
        /// The <see cref="RibbonBuilder"/>.
        /// </returns>
        private RibbonBuilder RibbonViewBuild()
        {
            var viewGroup = new RibbonGroupBuilder();
            viewGroup.AddGroup("Group Name");
            viewGroup.AddButton(this.iconProvider.GetIcon("Comment"), this.ShowDialogCommand);
            viewGroup.AddButton(this.iconProvider.GetIcon("Settings"), this.ShowDialogCommand);
            viewGroup.AddButton(this.iconProvider.GetIcon("FileTree"), this.ShowDialogCommand);

            var sectionView = new RibbonBuilder("View");
            sectionView.SetSection(viewGroup);
            return sectionView;
        }

        /// <summary>
        /// The ribbon view build.
        /// </summary>
        /// <returns>
        /// The <see cref="RibbonBuilder"/>.
        /// </returns>
        private RibbonBuilder RibbonViewBuild2()
        {
            var viewGroup = new RibbonGroupBuilder();
            viewGroup.AddGroup("View Group");
            viewGroup.AddButton(this.iconProvider.GetIcon("Comment"), this.ShowDialogCommand);
            viewGroup.AddButton(this.iconProvider.GetIcon("FileTree"), this.ShowDialogCommand);
            viewGroup.AddButton(this.iconProvider.GetIcon("Tag"), this.ShowDialogCommand);
            viewGroup.AddButton(this.iconProvider.GetIcon("FileTree"), this.ShowDialogCommand);


            var copyGroup = new RibbonGroupBuilder();
            copyGroup.AddGroup("View Group");
            copyGroup.AddButton(this.iconProvider.GetIcon("Plugin"), this.ShowDialogCommand);
            copyGroup.AddButton(this.iconProvider.GetIcon("TableColumn"), this.ShowDialogCommand);
            copyGroup.AddButton(this.iconProvider.GetIcon("Tag"), this.ShowDialogCommand);

            var sectionView = new RibbonBuilder("Launcher");
            sectionView.SetSection(viewGroup);
            sectionView.SetSection(copyGroup);
            return sectionView;
        }

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
