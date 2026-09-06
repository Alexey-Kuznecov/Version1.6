
using CommandSystem.Abstractions;
using System;
using System.Threading.Tasks;
using UnityCommander.Logging;
using UnityCommander.Logging.Contracts;
using UnityCommander.Logging.Core;
using UnityCommander.Services.Interfaces;
using UnityCommander.Services.Interfaces.Docking;

namespace UnityCommander.Modules.BottomPanel.Commands
{
    public sealed class ToolCommandProvider
    {
        private readonly IToolDockingManager _manager;
        private readonly IToolRegistry _registry;
        private readonly ILogger _logger;

        public ToolCommandProvider(
            IToolRegistry registry, 
            IToolDockingManager manager)
        {
            _manager = manager;
            _logger = Log.GetLoggerCreator().For<ToolCommandProvider>(LogScope.UserAction);
            _registry = registry;
        }

        public Task CreateTool(CommandContext ctx)
        {
            var toolId = ctx.Parameter?.ToString();

            if (string.IsNullOrWhiteSpace(toolId))
            {
                _logger.Info("Tool id is not specified.");
                return Task.CompletedTask;
            }

            var descriptor = _registry.Get(toolId);

            if (descriptor == null)
            {
                _logger.Info($"Tool '{toolId}' is not registered.");
                return Task.CompletedTask;
            }

            _manager.Create(descriptor);

            return Task.CompletedTask;
        }
    }
}
