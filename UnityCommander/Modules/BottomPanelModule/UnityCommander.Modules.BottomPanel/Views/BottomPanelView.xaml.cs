
using Prism.Ioc;
using System.Windows.Controls;
using UnityCommander.Services;
using UnityCommander.Services.Docking;
using UnityCommander.Services.Interfaces;
using UnityCommander.Services.Interfaces.Docking;

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

            var dockingService = ContainerLocator.Container.Resolve<IToolDockingService>() as ToolDockingService;
            dockingService?.SetDockingManager(this.ToolDockManager);
        }
    }
}
