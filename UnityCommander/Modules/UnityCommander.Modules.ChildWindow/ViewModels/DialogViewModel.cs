
namespace UnityCommander.Modules.ChildWindow.ViewModels
{
    using System;

    using Prism.Commands;
    using Prism.Mvvm;
    using Prism.Services.Dialogs;

    /// <summary>
    /// The view a view model.
    /// </summary>
    public class DialogViewModel : BindableBase, IDialogAware
    {
        /// <summary>
        /// The message.
        /// </summary>
        private string message;

        /// <summary>
        /// The title.
        /// </summary>
        private string title = "Notification";

        /// <summary>
        /// The _close dialog command.
        /// </summary>
        private DelegateCommand<string> closeDialogCommand;

        /// <summary>
        /// The request close.
        /// </summary>
        public event Action<IDialogResult> RequestClose;

        /// <summary>
        /// The close dialog command.
        /// </summary>
        public DelegateCommand<string> CloseDialogCommand =>
            this.closeDialogCommand ?? (this.closeDialogCommand = new DelegateCommand<string>(this.CloseDialog));

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public string Message
        {
            get => message;
            set => this.SetProperty(ref message, value);
        }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Title
        {
            get => this.title;
            set => this.SetProperty(ref this.title, value);
        }

        /// <summary>
        /// The raise request close.
        /// </summary>
        /// <param name="dialogResult">
        /// The dialog result.
        /// </param>
        public virtual void RaiseRequestClose(IDialogResult dialogResult)
        {
            RequestClose?.Invoke(dialogResult);
        }

        /// <summary>
        /// The can close dialog.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public virtual bool CanCloseDialog()
        {
            return true;
        }

        /// <summary>
        /// The on dialog closed.
        /// </summary>
        public virtual void OnDialogClosed()
        {
        }

        /// <summary>
        /// The on dialog opened.
        /// </summary>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        public virtual void OnDialogOpened(IDialogParameters parameters)
        {
            Message = parameters.GetValue<string>("message");
        }

        /// <summary>
        /// The close dialog.
        /// </summary>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        protected virtual void CloseDialog(string parameter)
        {
            ButtonResult result = ButtonResult.None;

            if (parameter?.ToLower() == "true")
            {
                result = ButtonResult.OK;
            }
            else if (parameter?.ToLower() == "false")
            {
                result = ButtonResult.Cancel;
            }

            this.RaiseRequestClose(new DialogResult(result));
        }
    }
}
