
using Prism.Ioc;
using System.Windows.Controls;
using UnityCommander.Services.Docking;

namespace UnityCommander.Modules.BottomPanel.Views
{
    /// <summary>
    /// Interaction logic for ViewA.xaml
    /// </summary>
    public partial class BottomPanelView : UserControl
    {
        public BottomPanelView()
        {
            InitializeComponent();

            var dockingContext = ContainerLocator.Container.Resolve<DockingContext>();

            dockingContext.ToolManager = this.ToolDockManager;
        }
    }
}
