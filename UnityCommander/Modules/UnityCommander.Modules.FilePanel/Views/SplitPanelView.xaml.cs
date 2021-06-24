#if NET472
using System.Windows.Markup;
[assembly: XmlnsDefinition("net472", "Namespace")]
#endif
namespace UnityCommander.Modules.FilePanel.Views
{
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for LeftPanel
    /// </summary>
    public partial class SplitPanelView : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SplitPanelView"/> class.
        /// </summary>
        public SplitPanelView()
        {
            this.InitializeComponent();
        }
    }
}
