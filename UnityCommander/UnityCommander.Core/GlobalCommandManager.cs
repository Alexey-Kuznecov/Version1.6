
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
                Delegate = @delegate,
                Source = type
            };

            this.globalCommands.Add(cmd);
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
            => this.globalCommands.SingleOrDefault(
                   cmd => cmd is IPluginCommand && cmd.Name == commandName)
               ?? this.globalCommands.Single(cmd => cmd.Name == commandName);

        public List<IGlobalCommand> GetCommands() => this.globalCommands;

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
                var cmd = default(IGlobalCommand);

                if (declaringType != method.DeclaringType)
                {
                    cmd = new PluginCommand
                    {
                        Name = att.Name,
                        Command = command,
                        ShortcutKey = att.Hotkey
                    };
                }
                else
                {
                    cmd = new GlobalCommand
                    {
                        Name = att.Name,
                        Command = command,
                        Delegate = action,
                        Source = type,
                        ShortcutKey = att.Hotkey
                    };
                }

                // var input = new InputBinding(cmd.Command, cmd.ShortcutKey);
                // var inputBindingCollection = new InputBindingCollection();
                // inputBindingCollection.Add(input);
                this.globalCommands.Add(cmd);
            }
        }
    }
}
