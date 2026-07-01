
using AvalonDock;
using AvalonDock.Core;
using AvalonDock.Layout;
using AvalonDock.Serializer.Xml;
using AvalonDock.Themes;
using Prism.Ioc;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
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
