using IconBrowser.Views;
using PluginSystem.Abstractions.Plugin;
using System.Threading.Tasks;
using UnityCommander.Abstractions.Dialog;

namespace IconBrowser.Commands
{
    internal class OpenIconMakerWindowCommand : IPluginCommand
    {
        public async Task ExecuteAsync(IPluginContext context)
        {
            var window = context.Services.Get<IWindowManager>();

            window.ShowDialog<IconMakerView>();
        }
    }
}