
namespace UnityCommander.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using UnityCommander.Common.Commands;
    using UnityCommander.Core.Generators;

    public class GlobalCommandManager : IGlobalCommandManager
    {
        private readonly Queue<IGlobalCommand> globalCommands;

        private readonly string commandSelector = "Default";

        public GlobalCommandManager(Queue<IGlobalCommand> commands)
        {
            this.globalCommands = commands;
        }

        public void CreateCommand(BaseCommand command)
        {
            this.SetCommand(command);
        }

        public void CreateCommand(string commandName, object instance, Action<object> action)
        {
            // TODO: Optimize this piece of code.
            var c = globalCommands.SingleOrDefault(cmd => cmd.Name == commandName);

            if (c != null) return;

            var type = action.Method.DeclaringType;
            var @delegate = Delegate.CreateDelegate(DelegateTypeFactory.Create(action.Method), instance, action.Method);
            var command = new GlobalCommandExecute(action, instance);

            var cmd = new GlobalCommand
            {
                Name = commandName,
                Command = command,
                CommandParameter = instance,
                ShortcutKey = null,
                Delegate = @delegate
            };

            this.globalCommands.Enqueue(cmd);
        }

        /// <summary>
        /// The get command.
        /// </summary>
        /// <param name="commandName">
        /// The command name.
        /// </param>
        /// <returns>
        /// The <see cref="IGlobalCommand"/>.
        /// </returns>
        public IGlobalCommand GetCommand(string commandName)
        {
            var commandSelected = default(GlobalCommand);

            foreach (var globalCommand in this.globalCommands.Where(
                         p=> ((GlobalCommandExecute)p.Command).Command.Method.Name.Contains(commandName)))
            {
                if (globalCommand is not GlobalCommand command) continue;
                if (!globalCommand.Name.Contains(this.commandSelector))
                {
                    commandSelected = command;
                    continue;
                }

                return globalCommand;
            }

            return commandSelected;
        } 

        public Queue<IGlobalCommand> GetCommands() => this.globalCommands;

        private void SetCommand(object instance)
        {
            Type type = instance.GetType();
            MethodInfo[] methods = type.GetMethods();
            
            foreach (var method in methods)
            {
                var declaringType = method.GetBaseDefinition().DeclaringType;
                if (Attribute.GetCustomAttribute(method, typeof(GlobalCommandAttribute)) is not GlobalCommandAttribute att) continue;

                var action = Delegate.CreateDelegate(DelegateTypeFactory.Create(method), instance, method);
                var command = new GlobalCommandExecute(action, type);
                this.globalCommands.Enqueue(
                    new GlobalCommand
                    {
                        Name = att.Name,
                        Command = command,
                        Delegate = action,
                        ShortcutKey = att.Hotkey,
                        OverrideCommand = GetOverriddenMethodInfo(method)
                    });
            }
        }

        private static MethodInfo GetOverriddenMethodInfo(MethodInfo methodOverride)
        {
            const BindingFlags Flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod;
            var baseType = methodOverride.GetBaseDefinition().DeclaringType;
            return baseType?.GetMethods(Flags).FirstOrDefault(methodVirtual => methodVirtual.Name == methodOverride.Name);
        }
    }
}
