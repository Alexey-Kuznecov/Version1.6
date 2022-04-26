using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using Prism.Mvvm;
using UnityCommander.Common;
using UnityCommander.Core.Generators;
using UnityCommander.Core.IO.Operations;
using UnityCommander.Integration.Commands;

namespace UnityCommander.Core
{
    public class GlobalCommandProvider : IGlobalCommandProvider
    {
        private static readonly List<GlobalCommand> GlobalCommands = new ();
        
        private static readonly GlobalCommandManager GlobalCommandManager = new (GlobalCommands);

        public GlobalCommandProvider()
        {
            GlobalCommandManager.CreateCommand(new FileManager());
        }

        public IGlobalCommandManager GetCommandManager() => GlobalCommandManager;

        private void SetCommand<T>(object instance)
        {
            if (GlobalCommands.Count != 0) return;

            if (instance is not T) return;

            Type type = instance.GetType();
            MethodInfo[] methods = type.GetMethods();

            foreach (var method in methods)
            {
                var att = Attribute.GetCustomAttribute(method, typeof(GlobalCommandAttribute));

                if (att != null)
                {
                    var action = Delegate.CreateDelegate(DelegateTypeFactory.Create(method), instance, method);
                    var cmd = new GlobalCommand(
                        ((GlobalCommandAttribute)att).Name, 
                        new GlobalCommandExecute<T>(action),
                                    new KeyGesture(Key.D, ModifierKeys.Control), 
                                        action,
                                            source:typeof(T));
                    
                    var input = new InputBinding(cmd.Command, cmd.ShortcutKey);
                    var inputBindingCollection = new InputBindingCollection();
                    inputBindingCollection.Add(input);
                    GlobalCommands.Add(cmd);
                }
            }
        }

        internal static GlobalCommand FindCommand(string commandName) 
            => GlobalCommands.Single(c => c.Name == commandName);
    }
}
