
using UnityCommander.Abstractions.Dialog;

namespace UnityCommander.Core.Commands
{
    public sealed class ShowSettingsCommand
    {
        private readonly IWindowManager _windowManager;

        public ShowSettingsCommand(IWindowManager windowManager)
        {
            _windowManager = windowManager;
        }

        public void Execute()
        {
            _windowManager.ShowModalDialog("Settings");
        }
    }
}
