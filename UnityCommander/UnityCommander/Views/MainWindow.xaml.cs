using Prism.Ioc;

namespace UnityCommander.Views
{
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using UnityCommander.Services.Interfaces;
    using Xceed.Wpf.AvalonDock;
    using Xceed.Wpf.AvalonDock.Controls;
    using Xceed.Wpf.AvalonDock.Layout;

    /// <summary>  
    /// The main window.  
    /// </summary>  
    public partial class MainWindow : Window
    {
        /// <summary>  
        /// Initializes a new instance of the <see cref="MainWindow"/> class.  
        /// </summary>  
        public MainWindow()
        {
            this.InitializeComponent();

            var dockingService = ContainerLocator.Container.Resolve<IDockingService>() as DockingService;
            dockingService?.SetDockingManager(this.DockManager);
        }
    }
}
