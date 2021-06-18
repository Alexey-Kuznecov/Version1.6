
namespace UnityCommander.Modules.ChildWindow.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using Prism.Commands;
    using Prism.Mvvm;

    /// <summary>
    /// The view a view model.
    /// </summary>
    public class ViewAViewModel : BindableBase
    {
        /// <summary>
        /// The message.
        /// </summary>
        private string message;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewAViewModel"/> class.
        /// </summary>
        public ViewAViewModel()
        {
            this.Message = "View A from your Prism Module";
        }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public string Message
        {
            get => this.message;
            set => this.SetProperty(ref this.message, value);
        }
    }
}
