#if NET472
using System.Windows.Markup;
[assembly: XmlnsDefinition("net472", "Namespace")]
#endif
namespace UnityCommander.Modules.FilePanel.Views
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for LeftPanel
    /// </summary>
    public partial class SplitPanelView : UserControl
    {
        /// <summary>
        /// The dependency property that will be bound to each controls
        /// in the navigation bar.
        /// </summary>
        private static readonly DependencyProperty OrientationPanelProperty;

        /// <summary>
        /// Initializes static members of the <see cref="SplitPanelView"/> class.
        /// </summary>
        static SplitPanelView()
        {
            OrientationPanelProperty = DependencyProperty.Register(
                "OrientationPanel",
                typeof(string),
                typeof(SplitPanelView),
                new FrameworkPropertyMetadata(string.Empty),
                ValidateValueCallback);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SplitPanelView"/> class.
        /// </summary>
        public SplitPanelView()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the command that will be bound to each control in the navigation bar.
        /// </summary>
        public string OrientationPanel
        {
            get => (string)GetValue(OrientationPanelProperty);
            set => this.SetValue(OrientationPanelProperty, value);
        }

        /// <summary>
        /// The validate value callback.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private static bool ValidateValueCallback(object value)
        {
            return true;
        }
    }
}
