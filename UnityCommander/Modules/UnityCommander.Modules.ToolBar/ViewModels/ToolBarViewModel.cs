

namespace UnityCommander.Modules.ToolBar.ViewModels
{
    using System.Windows.Controls;
    using System.Windows.Shapes;

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
        /// The icon.
        /// </summary>
        private Path icon;

        /// <summary>
        /// The user controls.
        /// </summary>
        private UserControl userControls;

        /// <summary>
        /// TODO The file operation tools.
        /// </summary>
        private Ribbon ribbonGroup;

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
            
            var editGroup = new RibbonGroupBuilder();
            editGroup.AddGroup("File Operation");
            editGroup.AddButton(iconProvider.GetIcon("Settings"), this.ShowDialogCommand);
            editGroup.AddButton(iconProvider.GetIcon("Settings"), this.ShowDialogCommand);
            editGroup.AddButton(iconProvider.GetIcon("Tag"), this.ShowDialogCommand);
            editGroup.AddButton(iconProvider.GetIcon("Tag"), this.ShowDialogCommand);

            var copyGroup = new RibbonGroupBuilder();
            copyGroup.AddGroup("View Group");
            copyGroup.AddButton(iconProvider.GetIcon("Comment"), this.ShowDialogCommand);
            copyGroup.AddButton(iconProvider.GetIcon("FileTree"), this.ShowDialogCommand);

            var sectionBuilder = new RibbonSectionBuilder("File");
            sectionBuilder.SetSection(editGroup);
            sectionBuilder.SetSection(copyGroup);

            var viewGroup = new RibbonGroupBuilder();
            viewGroup.AddGroup("View Group");
            viewGroup.AddButton(iconProvider.GetIcon("Comment"), this.ShowDialogCommand);
            viewGroup.AddButton(iconProvider.GetIcon("FileTree"), this.ShowDialogCommand);

            var sectionBuilder2 = new RibbonSectionBuilder("View");
            sectionBuilder2.SetSection(viewGroup);

            this.RibbonGroup = sectionBuilder.Build();
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
        public Ribbon RibbonGroup
        {
            get => this.ribbonGroup;
            set => this.SetProperty(ref this.ribbonGroup, value);
        }

        /// <summary>
        /// The show dialog command.
        /// </summary>
        public DelegateCommand ShowDialogCommand =>
            this.showDialogCommand ?? new DelegateCommand(this.ExecuteShowDialogCommand);

        /// <summary>
        /// The set ribbon.
        /// </summary>
        public DelegateCommand<object> SetRibbon => new DelegateCommand<object>(
            (obj) =>
        {
            if (obj is Button bt)
            {
                switch (bt.Content)
                {
                    case "Main":
                        this.UserControls = new MainTabControl();
                        this.UserControls.DataContext = new MainTabViewModel(this.iconProvider);
                        break;
                    case "View":
                        this.UserControls = new FileTabControl();
                        this.UserControls.DataContext = new FileTabViewModel();
                        break;
                    default:
                        this.UserControls = new AppTabControl();
                        this.UserControls.DataContext = new AppTabViewModel();
                        break;
                }
            }
        });

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
