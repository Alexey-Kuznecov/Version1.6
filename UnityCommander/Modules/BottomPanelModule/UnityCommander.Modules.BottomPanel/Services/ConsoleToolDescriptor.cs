
using System.Windows.Controls;
using UnityCommander.Modules.BottomPanel.Console;
using UnityCommander.Modules.BottomPanel.ViewModels;
using UnityCommander.Modules.BottomPanel.Views;
using UnityCommander.Services.Interfaces;
using UnityCommander.Services.Interfaces.Docking;

namespace UnityCommander.Modules.BottomPanel.Services
{
    public sealed class ConsoleToolDescriptor : IToolDescriptor
    {
        private readonly IConsoleManager _consoleManager;

        public string Id => "Console";
        public string Title => "Console";

        public bool CanCreateMultiple => true;

        public ToolDockSide DockSide 
            => ToolDockSide.Center;

        public ConsoleToolDescriptor(IConsoleManager consoleManager)
        {
            _consoleManager = consoleManager;
        }

        public Control Create()
        {
            var session = _consoleManager.Create();

            return new ConsoleView
            {
                DataContext = new ConsoleViewModel(session)
            };
        }
    }
}
