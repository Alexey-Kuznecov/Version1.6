
using Microsoft.Extensions.DependencyInjection;
using PluginSystem.Abstractions.Plugin;
using System.Threading.Tasks;
using UnityCommander.Abstractions.Command;

namespace UnityCommander.Core.Plugin
{
    public class PluginCommandDispatcher : IPluginCommandDispatcher
    {
        private readonly IPluginCommandRegistry _registry;

        private readonly PluginHost _host;

        public PluginCommandDispatcher(
            IPluginCommandRegistry registry,
            PluginHost host)
        {
            _registry = registry;
            _host = host;
        }

        public async Task ExecuteAsync(string commandId)
        {
            if (!_registry.TryGet(commandId, out var def))
                return;

            var plugin = _host.Get(def.OwnerId);

            var provider = plugin.Services;

            var command =
                (IPluginCommand)ActivatorUtilities
                    .CreateInstance(provider, def.CommandType);

            await command.ExecuteAsync(plugin.Context);
        }
    }
}
