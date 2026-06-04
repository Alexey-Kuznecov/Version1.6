
using Prism.Ioc;
using UnityCommander.CLI.Bootstrap;
using UnityCommander.CLI.Core;
using UnityCommander.CLI.Integration;
using UnityCommander.CLI.Integration.UnityCommander.CLI.Integration;
using UnityCommander.Commands;
using UnityCommander.Commands.Parsing;
using UnityCommander.Commands.Performance;
using UnityCommander.Commands.Rendering;
using UnityCommander.Commands.Services;
using UnityCommander.Common.Commands;
using UnityCommander.Modules.BottomPanel;
using UnityCommander.Services;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Dependencies
{
    public static class ConsoleCommandRegistration
    {
        public static void Register(IContainerRegistry registry)
        {
            // Ввод и вывод внутренней консоли приложения
            registry.RegisterSingleton<IConsoleInput, InternalConsoleInput>();
            registry.RegisterSingleton<IConsoleOutput, InternalConsoleOutput>();

            // Основные компоненты системы выполнения команд
            registry.RegisterSingleton<ConsoleCommandDispatcher>();
            registry.RegisterSingleton<ConsoleCommandFactory>();
            registry.RegisterSingleton<IConsoleCommandRegistry, ConsoleCommandRegistry>();
            registry.RegisterSingleton<IConsoleCommandInvoker, ConsoleCommandInvoker>();

            // Сервисы, предоставляющие команды приложению
            registry.RegisterSingleton<IConsoleCommandProvider, ConsoleCommandProvider>();

            // Сервисы, используемые командами внутренней консоли
            registry.RegisterSingleton<ISysStatService, SysStatService>();
            registry.RegisterSingleton<IProcessOpenFilesService, ProcessOpenFilesService>();

            // Вспомогательные компоненты инфраструктуры команд
            registry.RegisterSingleton<ICommandArgumentParser, CommandArgumentParser>();
            registry.RegisterSingleton<IConsoleRenderer<SystemStats>, SystemStatsRenderer>();

            var commands =
                ConsoleCommandDiscovery.Discover(
                    typeof(EchoCommand).Assembly);

            // Регистрация всех обнаруженных консольных команд
            foreach (var type in commands)
            {
                registry.Register(
                    typeof(IConsoleCommandBase),
                    type);
            }
        }
    }
}
