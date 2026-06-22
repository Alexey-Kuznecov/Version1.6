
using CommandSystem.Abstractions;
using CommandSystem.Core.UndoRedo;
using CommandSystem.Gui.Integraion;
using CommandSystem.Infrastructure.Execution;
using CommandSystem.Infrastructure.Lifecycle;
using Prism.Ioc;
using UnityCommander.Common.Commands;
using UnityCommander.Core.Commands;
using UnityCommander.Services;
using UnityCommander.Services.Command;
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
            registry.RegisterSingleton<ICommandFactory, CommandSystem.Core.Factory.CommandFactory>();
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
            registry.RegisterSingleton<CommandExecutionService>();
            registry.RegisterSingleton<CommandRegistryService>();

            registry.RegisterSingleton<IMultiCommandService, MultiCommandService>();
            registry.RegisterSingleton<ICommandUIService, CommandUIService>();
            registry.RegisterSingleton<ICommandExecuter, CommandExecuter>();
        }
    }
}
