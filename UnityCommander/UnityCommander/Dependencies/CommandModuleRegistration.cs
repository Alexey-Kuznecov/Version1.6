
using CommandSystem.Abstractions;
using CommandSystem.Core.Factory;
using CommandSystem.Core.UndoRedo;
using CommandSystem.Gui.Integraion;
using CommandSystem.Infrastructure.Execution;
using CommandSystem.Infrastructure.Lifecycle;
using Prism.Ioc;
using UnityCommander.Common.Commands;
using UnityCommander.Core;
using UnityCommander.Services;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Dependencies
{
    public static class CommandRegistration
    {
        public static void Register(IContainerRegistry registry)
        {
            // -------------------------------
            // 1. Регистрация компонентов командной системы
            // -------------------------------
            registry.RegisterSingleton<ICommandRegistry, CommandRegistry>();
            registry.RegisterSingleton<ICommandFactory, CommandFactory>();
            registry.RegisterSingleton<ICommandExecutor, CommandExecutor>();
            registry.RegisterSingleton<ICommandDispatcher, CommandDispatcher>();
            registry.RegisterSingleton<IHistoryStore, InMemoryHistoryStore>();
            registry.RegisterSingleton<IHistoryManager, CommandHistoryManager>();

            // -------------------------------
            // 2. Регистрация GUI-команд
            // -------------------------------
            registry.RegisterSingleton<ICommandRegister, GuiCommandRegister>();
            registry.RegisterSingleton<IGuiCommandExecutor, GuiCommandExecuter>();
            registry.RegisterSingleton<IGuiCommandProvider, GuiCommandProvider>();
            registry.RegisterSingleton<CommandPresentationProvider>();
            registry.RegisterSingleton<CommandService>();

            registry.RegisterSingleton<IMultiCommandService, MultiCommandService>();
            registry.RegisterSingleton<ICommandUIService, CommandUIService>();
        }
    }
}
