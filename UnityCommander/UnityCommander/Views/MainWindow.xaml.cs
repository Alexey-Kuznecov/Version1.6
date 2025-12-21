using CommandSystem.Abstractions;
using Prism.Ioc;
using System.Windows;
using UnityCommander.Commands;
using UnityCommander.Services;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Views
{
    /// <summary>  
    /// The main window.  
    /// </summary>  
    public partial class MainWindow : Window
    {
        public static readonly DependencyProperty IsBottomPanelVisibleProperty =
            DependencyProperty.Register(
            nameof(IsBottomPanelVisible),
            typeof(bool),
            typeof(MainWindow),
            new PropertyMetadata(true));

        public bool IsBottomPanelVisible
        {
            get => (bool)GetValue(IsBottomPanelVisibleProperty);
            set => SetValue(IsBottomPanelVisibleProperty, value);
        }
        /// <summary>  
        /// Initializes a new instance of the <see cref="MainWindow"/> class.  
        /// </summary>  
        public MainWindow()
        {
            this.InitializeComponent();

            var dockingService = ContainerLocator.Container.Resolve<IDockingService>() as DockingService;
            dockingService?.SetDockingManager(this.DockManager);
            this.RegisterCommands();
        }

        private void RegisterCommands()
        {
            var commandService = ContainerLocator.Container.Resolve<CommandService>();
            var shellProvider = new ShellCommandProvider(this);
            // Регистрируем базовые команды
            commandService.Register(
               new CommandMetadata("toggle.bottom.panel", "Скрывает/показывает нижнюю панель"),
               shellProvider.ToggleBottomPanel);
        }
    }
}
