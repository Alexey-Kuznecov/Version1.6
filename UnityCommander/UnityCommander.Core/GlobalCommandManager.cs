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
                    var att = Attribute.GetCustomAttribute(method, typeof(GlobalCommandAttribute));
                    
                    if (att != null)
                    {
                        var action = Delegate.CreateDelegate(DelegateTypeFactory.Create(method), instance, method);
                        var command = new GlobalCommandExecute(action, type);

                        var cmd = new GlobalCommand(
                            ((GlobalCommandAttribute)att).Name,
                            command,
                            new KeyGesture(Key.D, ModifierKeys.Control),
                            action,
                            source: type);

                        var input = new InputBinding(cmd.Command, cmd.ShortcutKey);
                        var inputBindingCollection = new InputBindingCollection();
                        inputBindingCollection.Add(input);
                        globalCommands.Add(cmd);
                    }
                }
            }
        }

        public ICommand GetCommand(string commandName) 
            => this.globalCommands.Single(c => c.Name == commandName).Command;
        
    }
}
