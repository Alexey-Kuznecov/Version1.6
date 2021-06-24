
namespace IconBrowser
{
    using System.Windows.Controls;
    using UnityCommander.Integration.Contracts;
    using UnityCommander.Integration.Dialog;

    /// <summary>
    /// The icon browser control.
    /// </summary>
    public partial class IconBrowserControl : UserControl, IDialogService, IPluginDescriptor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IconBrowserControl"/> class.
        /// </summary>
        public IconBrowserControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets plugin name.
        /// </summary>
        public string DisplayName { get; set; } = "Icon Browser for UC";

        /// <summary>
        /// Gets or sets plugin description.
        /// </summary>
        public string Description { get; set; } = "Plugin for management and viewing the collection of icons.";
    }
}
