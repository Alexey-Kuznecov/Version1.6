using CommandSystem.Abstractions;
using Prism.Ioc;
using System.Windows;
using UnityCommander.Commands;
using UnityCommander.Common.Commands;
using UnityCommander.Services;
using UnityCommander.Services.Docking;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Views
{
    /// <summary>  
    /// The main window.  
    /// </summary>  
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();

            var dockingService = ContainerLocator.Container.Resolve<IDockingService>() as DockingService;
            dockingService?.SetDockingManager(this.DockManager);
        }
    }
}
