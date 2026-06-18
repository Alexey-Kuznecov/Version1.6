
using AdvancedCopyFiles.Views;
using PluginSystem.Abstractions.Plugin;
using UnityCommander.Abstractions.Dialog;

namespace UnityCommander.Core.Plugin
{
    public class OpenCopyWindowCommand : IPluginCommand
    {
        public async Task ExecuteAsync(IPluginContext context)
        {
            var window = context.Services.Get<IWindowManager>();

            window.ShowDialog<MainView>();
        }
    }
}
