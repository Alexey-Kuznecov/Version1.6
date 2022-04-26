using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using UnityCommander.Common;
using UnityCommander.Core.Generators;
using UnityCommander.Integration.Commands;

namespace UnityCommander.Core
{
    public class GlobalCommandManager : IGlobalCommandManager
    {
        private readonly List<GlobalCommand> globalCommands;

        public GlobalCommandManager(List<GlobalCommand> commands)
        {
            this.globalCommands = commands;
        }

        public void CreateCommand(BaseCommand command)
        {
            this.SetCommand(command);
        }

        private void SetCommand(object instance)
        {
            Type type = instance.GetType();
            MethodInfo[] methods = type.GetMethods();
            
            foreach (var method in methods)
            {
                if (method.GetBaseDefinition().DeclaringType != method.DeclaringType)
                {
                    var att = Attribute.GetCustomAttribute(method, typeof(GlobalCommandAttribute)) as GlobalCommandAttribute;

                    if (att == null) continue;

                    var action = Delegate.CreateDelegate(DelegateTypeFactory.Create(method), instance, method);
                    var command = new GlobalCommandExecute(action, type);

                    var cmd = new GlobalCommand
                    {
                        CommandName = att.Name,
                        CommandSource = att.Source,
                        Command = command,
                        ShortcutKey = new KeyGesture(Key.D, ModifierKeys.Control),
                        Delegate = action,
                        Source = type
                       
                    };

                    //var input = new InputBinding(cmd.Command, cmd.ShortcutKey);
                    //var inputBindingCollection = new InputBindingCollection();
                    //inputBindingCollection.Add(input);
                    globalCommands.Add(cmd);
                }
            }
        }

        public GlobalCommand GetCommand(string commandName)
            => this.globalCommands.SingleOrDefault(
                   cmd=> cmd.CommandSource == CommandSource.Plugin && cmd.CommandName == commandName) 
                        ?? this.globalCommands.Single(cmd => cmd.CommandName == commandName);
    }
}
