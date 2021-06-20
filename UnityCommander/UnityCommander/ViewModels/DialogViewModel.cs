
namespace UnityCommander.ViewModels
{
    using System;
    using System.Windows.Controls;

    using Prism.Commands;
    using Prism.Mvvm;
    using Prism.Services.Dialogs;

    using UnityCommander.Core.Mvvm;

    /// <summary>
    /// The dialog view model.
    /// </summary>
    public class DialogViewModel : BindableBase, IDialogAware
    {
        /// <summary>
        /// The close dialog command.
        /// </summary>
        public DelegateCommand CloseDialogCommand =>
            this.closeDialogCommand ?? (this.closeDialogCommand = new DelegateCommand(this.ExecuteCloseDialogCommand));


        /// <summary>
        /// The close dialog command.
        /// </summary>
        private DelegateCommand closeDialogCommand;


        /// <summary>
        /// The control.
        /// </summary>
        private UserControl control;

        /// <summary>
        /// The request close.
        /// </summary>
        public event Action<IDialogResult> RequestClose;

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
        public string Title { get; } = "My Dialog";

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
            this.UserControl = param?.Package as UserControl;
        }

        /// <summary>
        /// The execute close dialog command.
        /// </summary>
        private void ExecuteCloseDialogCommand()
        {
            this.RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
        }
    }
}
