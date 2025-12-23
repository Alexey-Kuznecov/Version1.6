using Microsoft.Extensions.DependencyInjection;
using PluginSystem.Abstractions.Plugin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityCommander.CLI.Core;
using UnityCommander.CLI.Integration;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Commands
{
    [ConsoleCommand("plugin", "Управление плагинами", "pl")]
    public class PluginConsoleCommand : IConsoleCommand
    {
        private readonly IPluginProvider _pluginProvider;
        public string Name => "plugin";
        public IEnumerable<string> Aliases => ["pl"];
        public string Description => "Управление плагинами";

        public PluginConsoleCommand(IPluginProvider pluginProvider)
        {
            _pluginProvider = pluginProvider;
        }

        public Task ExecuteAsync(
            IConsoleCommandContext context,
            CancellationToken cancellationToken = default)
        {
            var args = context.Arguments.ToArray();

            if (args.Length == 0)
            {
                PrintHelp(context);
                return Task.CompletedTask;
            }
            
            var action = args[0].ToLowerInvariant();
            
            return action switch
            {
                "load" => Load(context, args.Skip(1).ToArray()),
                "unload" => Unload(context, args.Skip(1).ToArray()),
                "reload" => Reload(context, args.Skip(1).ToArray()),
                "list" => List(context),
                "info" => Info(context, args.Skip(1).ToArray()),
                _ => Unknown(context, action)
            };
        }

        public Task FinalizeAsync() => Task.CompletedTask;

        private Task Load(IConsoleCommandContext context, string[] args)
        {
            if (args.Contains("--all"))
            {
                var loaded = _pluginProvider.LoadAll();
                context.Output.WriteLine($"Загружено плагинов: {loaded.Count()}");
                return Task.CompletedTask;
            }

            if (args.Length == 0)
            {
                context.Output.WriteError("Укажите имя или путь к плагину.");
                return Task.CompletedTask;
            }

            var target = Path.Combine(AppContext.BaseDirectory, "Plugins", Path.GetFileNameWithoutExtension(args[0]), args[0]);

            if (_pluginProvider.Load(target))
                context.Output.WriteLine($"Плагин загружен: {args[0]}");
            else
                context.Output.WriteError($"Не удалось загрузить плагин: {args[0]}");

            return Task.CompletedTask;
        }

        private Task Unload(IConsoleCommandContext context, string[] args)
        {
            var pluginsRoot = Path.Combine(AppContext.BaseDirectory, "Plugins");

            if (args.Contains("--all"))
            {
                var plugins = _pluginProvider.GetAll().ToList();
                foreach (var plugin in plugins)
                    _pluginProvider.Unload(plugin.PluginInfo.Name);

                context.Output.WriteLine("Все плагины выгружены.");
                return Task.CompletedTask;
            }

            if (args.Length == 0)
            {
                context.Output.WriteError("Укажите имя плагина.");
                return Task.CompletedTask;
            }

            var name = args[0];

            if (_pluginProvider.Unload(name))
                context.Output.WriteLine($"Плагин выгружен: {name}");
            else
                context.Output.WriteError($"Плагин не найден или не выгружен: {name}");

            return Task.CompletedTask;
        }

        private Task Reload(IConsoleCommandContext context, string[] args)
        {
            if (args.Length == 0)
            {
                context.Output.WriteError("Укажите имя плагина.");
                return Task.CompletedTask;
            }

            var name = args[0];

            _pluginProvider.Unload(name);

            if (_pluginProvider.Load(name))
                context.Output.WriteLine($"Плагин перезагружен: {name}");
            else
                context.Output.WriteError($"Не удалось перезагрузить плагин: {name}");

            return Task.CompletedTask;
        }

        private Task List(IConsoleCommandContext context)
        {
            var plugins = _pluginProvider.GetAll().ToList();

            if (plugins.Count == 0)
            {
                context.Output.WriteLine("Плагины не загружены.");
                return Task.CompletedTask;
            }

            foreach (var plugin in plugins)
            {
                context.Output.WriteLine(
                    $"{plugin.PluginInfo.Name} | v{plugin.PluginInfo.Version} | {plugin.PluginInfo.Description}");
            }

            return Task.CompletedTask;
        }

        private Task Info(IConsoleCommandContext context, string[] args)
        {
            if (args.Length == 0)
            {
                context.Output.WriteError("Укажите имя плагина.");
                return Task.CompletedTask;
            }

            var info = _pluginProvider.GetInfo(args[0]);

            if (info == null)
            {
                context.Output.WriteError("Плагин не найден.");
                return Task.CompletedTask;
            }

            context.Output.WriteLine($"Имя: {info.Name}");
            context.Output.WriteLine($"Версия: {info.Version}");
            context.Output.WriteLine($"Описание: {info.Description}");

            return Task.CompletedTask;
        }

        private Task Unknown(IConsoleCommandContext context, string action)
        {
            context.Output.WriteError($"Неизвестная команда: {action}");
            PrintHelp(context);
            return Task.CompletedTask;
        }

        private void PrintHelp(IConsoleCommandContext context)
        {
            context.Output.WriteLine("Использование:");
            context.Output.WriteLine("  plugin load <name|path>");
            context.Output.WriteLine("  plugin load --all");
            context.Output.WriteLine("  plugin unload <name>");
            context.Output.WriteLine("  plugin unload --all");
            context.Output.WriteLine("  plugin reload <name>");
            context.Output.WriteLine("  plugin list");
            context.Output.WriteLine("  plugin info <name>");
        }
    }
}