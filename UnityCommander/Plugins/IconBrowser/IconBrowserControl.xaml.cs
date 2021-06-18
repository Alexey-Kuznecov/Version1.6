
namespace IconBrowser
{
    using System.Windows.Controls;

    using UnityCommander.Integration.Dialog;

    /// <summary>
    /// The icon browser control.
    /// </summary>
    public partial class IconBrowserControl : UserControl, IDialogService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IconBrowserControl"/> class.
        /// </summary>
        public IconBrowserControl()
        {
            InitializeComponent();
        }
    }
}
