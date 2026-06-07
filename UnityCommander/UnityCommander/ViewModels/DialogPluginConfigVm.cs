
namespace UnityCommander.ViewModels
{
    using System;
    using System.Windows.Controls;

    using Prism.Commands;
    using Prism.Dialogs;
    using Prism.Mvvm;

    using UnityCommander.Core.Mvvm;
    using UnityCommander.Services.Interfaces;

    /// <summary>
    /// The dialog plugin config vm.
    /// </summary>
    internal class DialogPluginConfigVm : BindableBase, IDialogAware
    {
        /// <summary>
        /// The close dialog command.
        /// </summary>
        private DelegateCommand closeDialogCommand;

        /// <summary>
        /// The control.
        /// </summary>
        private UserControl control;

        private IPluginProvider pluginLoader;

        /// <summary>
        /// Initializes a new instance of the <see cref="DialogPluginConfigVm"/> class.
        /// </summary>
        /// <param name="pluginLoader">
        /// The plugin loader.
        /// </param>
        public DialogPluginConfigVm(IPluginProvider pluginLoader)
        {
            this.pluginLoader = pluginLoader;
        }

        /// <summary>
        /// The request close.
        /// </summary>
        public DialogCloseListener RequestClose { get; private set; }

        /// <summary>
        /// The close dialog command.
        /// </summary>
        public DelegateCommand CloseDialogCommand =>
            this.closeDialogCommand ??= new DelegateCommand(this.ExecuteCloseDialogCommand);
        
        /// <summary>
        /// Gets or sets the user control.
        /// </summary>
        public UserControl UserControl
        {
            get => this.control;
            set => this.SetProperty(ref this.control, value);
        }

        /// <summary>
        /// Gets the title.
        /// </summary>
        public string Title => "My Dialog";

        /// <summary>
        /// The can close dialog.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool CanCloseDialog()
        {
            return true;
        }

        /// <summary>
        /// The on dialog closed.
        /// </summary>
        public void OnDialogClosed()
        {
        }

        /// <summary>
        /// The on dialog opened.
        /// </summary>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        public void OnDialogOpened(IDialogParameters parameters)
        {
            var param = parameters as OverrideDialogParameters;
            var type = param?.Package.GetType();

            //foreach (var attribute in type.GetPluginConfigs())
            //{
            //}
        }

        /// <summary>
        /// The execute close dialog command.
        /// </summary>
        private void ExecuteCloseDialogCommand()
        {
            RequestClose.Invoke(new DialogResult(ButtonResult.OK));
        }

        /// <summary>
        /// The unload plugin.
        /// </summary>
        private void UnloadPlugin()
        {
            this.pluginLoader = null;
        }
    }
}
