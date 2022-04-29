
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
        
        private readonly Queue<IGlobalCommand> virtualGlobalCommands = new ();

        private readonly Queue<IGlobalCommand> globalCommandsConflicts;

        private readonly Queue<MethodInfo> methodInfos = new ();

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
            => this.globalCommands.SingleOrDefault(
                   cmd => cmd is IPluginCommand && cmd.Name == commandName)
               ?? this.globalCommands.Single(cmd => cmd.Name == commandName);

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
                methodInfos.Enqueue(method);
                
                if (method.IsVirtual && declaringType != method.DeclaringType)
                {
                    this.globalCommands.Enqueue(
                        new GlobalCommand
                        {
                            Name = att.Name,
                            Command = command,
                            Delegate = action,
                            Source = type,
                            DeclareType = method.GetBaseDefinition(),
                            ShortcutKey = att.Hotkey
                        });
                    return;
                }
                
                //if (method.IsVirtual && declaringType == method.DeclaringType)
                //{
                //    this.virtualGlobalCommands.Enqueue(
                //        new GlobalCommand
                //        {
                //            Name = att.Name,
                //            //Command = command,
                //            //Delegate = action,
                //            Source = type,
                //            ShortcutKey = att.Hotkey
                //        });
                //    return;
                //}

                foreach (var virtualGlobalCommand in this.virtualGlobalCommands)
                {
                    //var baseMethod = ((GlobalCommand)virtualGlobalCommand).Delegate.Method;
                    //var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod;
                    //var m = method.DeclaringType.GetMethods(flags).FirstOrDefault(i => baseMethod.IsBaseMethodOf(i));
                    //var dd = this.globalCommands.FirstOrDefault(globalCommand => ((GlobalCommandExecute)globalCommand.Command).Command.Method
                    //         != baseMethod);

                    //if (dd != null)
                    //{
                    //    var ddd = ((GlobalCommandExecute)dd.Command).Command.Method.GetBaseDefinition().DeclaringType;

                    //    if (ddd == method.GetBaseDefinition().DeclaringType)
                    //    {
                    //        return;
                    //    }
                    //}
                    
                    //if (method.DeclaringType.HasOverridingMethod(baseMethod))
                    //{
                    //    this.globalCommands.Enqueue(
                    //        new PluginCommand
                    //            {
                    //                Name = virtualGlobalCommand.Name,
                    //                //Command = command,
                    //                ShortcutKey = att.Hotkey
                    //            });
                    //}
                    //else
                    //{
                    //    this.globalCommands.Enqueue(virtualGlobalCommand);
                    //}
                }
            }
        }
    }

    public static class Methods 
    {
        public static bool HasOverridingMethod(this Type type, MethodInfo baseMethod)
        {
            return type.GetOverridingMethod(baseMethod) != null;
        }
        public static MethodInfo GetOverridingMethod(this Type type, MethodInfo baseMethod)
        {
            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod;
            return type.GetMethods(flags).FirstOrDefault(i => baseMethod.IsBaseMethodOf(i));
        }
        public static bool IsBaseMethodOf(this MethodInfo baseMethod, MethodInfo method)
        {
            return baseMethod.DeclaringType != method.DeclaringType && baseMethod == method.GetBaseDefinition();
        }
    }
}
