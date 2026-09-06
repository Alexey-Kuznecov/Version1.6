
using AdvancedCopyFiles.Views;
using PluginSystem.Abstractions.Plugin;
using UnityCommander.Abstractions.Dialog;

namespace AdvancedCopyFiles.Commands
{
    public class OpenCopyDialog : IPluginCommand
    {
        public async Task ExecuteAsync(IPluginContext context)
        {
            var window = context.Services.Get<IWindowManager>();

            window.ShowDialog<MainViewOld>();
        }
    }
}