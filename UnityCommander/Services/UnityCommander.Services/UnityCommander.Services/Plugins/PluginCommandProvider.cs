
using Microsoft.Extensions.DependencyInjection;
using PluginSystem.Abstractions.Plugin;
using System.Collections.Generic;
using UnityCommander.Abstractions.Command;
using UnityCommander.Core.Plugin;
using UnityCommander.Services.Interfaces.Plugins;

namespace UnityCommander.Services.Plugins
{
    public sealed class PluginCommandProvider : IPluginCommandProvider
    {
        private readonly IPluginCommandRegistry _registry;
        private readonly PluginHost _host;

        private readonly Dictionary<string, (IPluginCommand Command, IPluginContext Context)> _cache = new();

        public PluginCommandProvider(
            PluginHost host,
            IPluginCommandRegistry registry)
        {
            _host = host;
            _registry = registry;
        }

        public bool TryGet(
            string commandId,
            out (IPluginCommand Command, IPluginContext Context) result)
        {
            result = default;

            if (string.IsNullOrWhiteSpace(commandId))
                return false;

            if (_cache.TryGetValue(commandId, out result))
                return true;

            // 2. registry
            if (!_registry.TryGet(commandId, out var def))
                return false;

            var plugin = _host.Get(def.OwnerId);
            if (plugin == null)
                return false;

            var command = ActivatorUtilities.CreateInstance(
                plugin.Services,
                    def.CommandType) as IPluginCommand;

            if (command == null)
                return false;

            result = (command, plugin.Context);

            // 3. cache write
            _cache[commandId] = result;

            return true;
        }

        //public IPluginCommand Get(string commandId)
        //{
        //    if (TryGet(commandId, out var cmd))
        //        return cmd;

        //    throw new KeyNotFoundException($"Command not found: {commandId}");
        //}

        //public IReadOnlyCollection<string> GetAllIds()
        //    => _registry.GetAll();
    }
}
