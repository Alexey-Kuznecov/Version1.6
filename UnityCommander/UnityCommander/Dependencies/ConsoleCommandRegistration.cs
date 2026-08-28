
using Prism.Ioc;
using System;
using System.Diagnostics;
using UnityCommander.CLI.Bootstrap;
using UnityCommander.CLI.Core;
using UnityCommander.CLI.Integration;
using UnityCommander.CLI.Integration.UnityCommander.CLI.Integration;
using UnityCommander.CLI.Lifecicle;
using UnityCommander.Commands;
using UnityCommander.Modules.BottomPanel.Console;
using UnityCommander.Services;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Dependencies
{
    public static class ConsoleCommandRegistration
    {
        public static void Register(IContainerRegistry registry)
        {
            // Ввод и вывод внутренней консоли приложения
            //registry.RegisterSingleton<IConsoleInput, InternalConsoleInput>();
            //registry.RegisterSingleton<IConsoleOutput, InternalConsoleOutput>();

            registry.RegisterSingleton<ConsoleCommandLoop>();
            registry.RegisterSingleton<ConsoleInputProcessor>();
            registry.RegisterSingleton<ConsoleAutocompleteProcessor>();
            registry.RegisterSingleton<IConsoleManager, ConsoleManager>();
            registry.RegisterSingleton<IConsoleProfileStore, ConsoleProfileStore>();

            // Основные компоненты системы выполнения команд
            registry.RegisterSingleton<ConsoleCommandDispatcher>();
            registry.RegisterSingleton<ConsoleCommandFactory>();
            registry.RegisterSingleton<ConsoleApplicationLifetime>();
            registry.RegisterSingleton<CommandProcessManager>();
            registry.RegisterSingleton<ConsoleLineExecutor>();
            registry.RegisterSingleton<IConsoleCommandRegistry, ConsoleCommandRegistry>();
            registry.RegisterSingleton<IConsoleCommandInvoker, ConsoleCommandInvoker>();

            // Сервисы, предоставляющие команды приложению

            DiagnosticRegistration.Register(registry); //Выяснить почему диагностика вызывается здесь, а не в App.xaml.cs

            registry.RegisterSingleton<IConsoleCommandProvider, ConsoleCommandProvider>();

            var commands =
                ConsoleCommandDiscovery.Discover(
                    typeof(EchoCommand).Assembly);

            // Регистрация всех обнаруженных консольных команд
            foreach (var type in commands)
            {
                registry.RegisterSingleton(
                    typeof(IConsoleCommand),
                    type);
            }
        }
    }
}
