
// ReSharper disable All
namespace UnityCommander.Modules.ToolBar.ViewModels
{
    using System.Windows.Controls;
    using System.Windows.Input;

    using Prism.Commands;
    using Prism.Mvvm;
    using Prism.Services.Dialogs;

    using UnityCommander.Core.Mvvm;
    using UnityCommander.Modules.ToolBar.Views;
    using UnityCommander.Services.Interfaces;
    using UnityCommander.Common.Styling;

    /// <summary>
    /// The view a view model.
    /// </summary>
    public class ViewAViewModel : BindableBase
    {
        /// <summary>
        /// The dialog service.
        /// </summary>
        private readonly IDialogService dialogService;

        /// <summary>
        /// The plugin loader service.
        /// </summary>
        private readonly IPluginLoaderService pluginLoaderService;

        /// <summary>
        /// The _message.
        /// </summary>
        private string message;

        /// <summary>
        /// The user controls.
        /// </summary>
        private UserControl userControls;

        /// <summary>
        /// The show dialog command.
        /// </summary>
        private DelegateCommand showDialogCommand;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewAViewModel"/> class.
        /// </summary>
        /// <param name="dialogService">
        /// The dialog Service.
        /// </param>
        /// <param name="pluginLoaderService">
        /// The plugin Loader Service.
        /// </param>
        public ViewAViewModel(IDialogService dialogService, IPluginLoaderService pluginLoaderService)
        {
            this.dialogService = dialogService;
            this.pluginLoaderService = pluginLoaderService;
            this.Message = "This Toolbar View";
            this.UserControls = new MainTabControl();
        }

        /// <summary>
        /// The show dialog command.
        /// </summary>
        public DelegateCommand ShowDialogCommand =>
            this.showDialogCommand ?? (this.showDialogCommand = new DelegateCommand(this.ExecuteShowDialogCommand));

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
                        break;
                    case "View":
                        this.UserControls = new FileOperationControl();
                        break;
                    default:
                        this.UserControls = new MainTabControl();
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
            var dialogs = pluginLoaderService.GetDialogService();

            foreach (var dialog in dialogs)
            {
                this.dialogService.ShowDialog("DialogView", new OverrideDialogParameters(dialog), r => { });
            }
        }
    }
}
