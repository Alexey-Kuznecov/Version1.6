using Prism.Ioc;
using System.Windows;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Views
{
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
