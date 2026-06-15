
namespace UnityCommander.Modules.FilePanel.Views
{
    using System.Windows.Controls;
    using UnityCommander.Common.Module;

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

            Loaded += (_, __) =>
            {
                (DataContext as IViewAttachAware)?.OnViewAttached(this);
            };

            Unloaded += (_, __) =>
            {
                (DataContext as IViewAttachAware)?.OnViewDetached();
            };
        }
    }
}
