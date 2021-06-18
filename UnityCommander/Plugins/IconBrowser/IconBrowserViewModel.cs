
namespace IconBrowser
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using AkuzIcons.Mvvm.Base;

    /// <summary>
    /// The icon browser view model.
    /// </summary>
    public class IconBrowserViewModel : PropertiesChanged
    {
        /// <summary>
        /// The message.
        /// </summary>
        private string message;

        /// <summary>
        /// Initializes a new instance of the <see cref="IconBrowserViewModel"/> class.
        /// </summary>
        public IconBrowserViewModel()
        {
            this.Message = "Hello from Icon Browser";
        }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public string Message
        {
            get => this.message;
            set
            {
                this.message = value;
                this.OnPropertyChanged("Message");
            }
        }
    }
}
