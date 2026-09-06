
using PluginSystem.Abstractions.Plugin;
using System;
using System.Threading.Tasks;

namespace UnityCommander.Core.Plugin
{
    public static class PluginCommandFactory
    {
        public static PluginCommand Create(Action<IPluginContext> execute)
        {
            return new PluginCommand(ctx =>
            {
                execute(ctx);
                return Task.CompletedTask;
            });
        }

        public static PluginCommand Create(Func<IPluginContext, Task> execute)
        {
            return new PluginCommand(execute);
        }
    }
}
