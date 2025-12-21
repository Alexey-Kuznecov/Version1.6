
using CommandSystem.Abstractions;
using System;
using UnityCommander.Views;

namespace UnityCommander.Commands
{
    public class ShellCommandProvider
    {
        private readonly MainWindow _shell;

        public ShellCommandProvider(MainWindow shell)
        {
            _shell = shell;
        }

        public Action<CommandContext> ToggleBottomPanel => ctx =>
        {
            _shell.IsBottomPanelVisible = !_shell.IsBottomPanelVisible;
        };
    }
}
