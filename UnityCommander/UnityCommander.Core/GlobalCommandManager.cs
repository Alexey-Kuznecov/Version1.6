
namespace UnityCommander.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Windows.Input;
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

        public void CreateCommand(object command)
        {
            this.SetCommand(command);
        }

        public void CreateCommand(string commandName, object instance, Action<object> action)
        {
            // TODO: Optimize this piece of code.
            //var c = globalCommands.SingleOrDefault(cmd => cmd.Name == commandName);

            //if (c != null) return;

            var command = new GlobalCommandExecute(action, instance);

            var cmd = new GlobalCommand
            {
                Name = commandName,
                Command = command,
                CommandParameter = instance,
                ShortcutKey = null,
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

        public IEnumerable<IGlobalCommand> GetCommands(string commandName) 
            => this.globalCommands.Where(p => ((GlobalCommandExecute)p.Command).Command.Method.Name.Contains(commandName));

        public Queue<IGlobalCommand> GetCommands() => this.globalCommands;

        private void SetCommand(object instance)
        {
            Type type = instance.GetType();
            MethodInfo[] methods = type.GetMethods();
            
            foreach (var method in methods)
            {
                if (!method.CustomAttributes.Any(p => p.AttributeType.Name == "GlobalCommandAttribute")) continue;
             
                var action = Delegate.CreateDelegate(DelegateTypeFactory.Create(method), instance, method);
                var command = new GlobalCommandExecute(action, type);

                var globalCommand = new GlobalCommand
                {
                    Command = command
                };

                foreach (var attribute in method.GetCustomAttributes())
                {
                     var properties = attribute.GetType().GetProperties();
                     globalCommand.Name = (string)attribute.GetType().GetProperty("Name").GetValue(attribute);
                     globalCommand.ShortcutKey = (InputGesture)attribute.GetType().GetProperty("Hotkey").GetValue(attribute);
                }

                this.globalCommands.Enqueue(globalCommand);
            }
        }

        public void UpdateCommand(string commandName)
        {
            throw new NotImplementedException();
        }

        public void CreateSingletonCommand(string commandName, object args, Action<object> action)
        {
            // TODO: Optimize this piece of code.
            IGlobalCommand globalCommand = globalCommands.SingleOrDefault(cmd => cmd.Name == commandName);

            if (globalCommand != null)
            {
                globalCommand.CommandParameter = args;
                return;
            }

            var command = new GlobalCommandExecute(action, args);
            
            globalCommand = new GlobalCommand
            {
                Name = commandName,
                Command = command,
                CommandParameter = args,
                ShortcutKey = null,
            };
            
            this.globalCommands.Enqueue(globalCommand);
        }

        private static MethodInfo GetOverriddenMethodInfo(MethodInfo methodOverride)
        {
            const BindingFlags Flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod;
            var baseType = methodOverride.GetBaseDefinition().DeclaringType;
            return baseType?.GetMethods(Flags).FirstOrDefault(methodVirtual => methodVirtual.Name == methodOverride.Name);
        }
    }
}
