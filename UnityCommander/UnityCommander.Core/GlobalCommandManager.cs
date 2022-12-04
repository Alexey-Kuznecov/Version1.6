
namespace UnityCommander.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using System.Windows.Input;
    
    using UnityCommander.Common.Commands;
    using UnityCommander.Core.Generators;

    /// <summary>
    /// The global command manager.
    /// </summary>
    [SuppressMessage("ReSharper", "StyleCop.SA1503")]
    public class GlobalCommandManager : IGlobalCommandManager
    {
        /// <summary>
        /// The global commands.
        /// </summary>
        private readonly Queue<IGlobalCommand> globalCommands;

        /// <summary>
        /// 
        /// </summary>
        private readonly HashSet<IGlobalCommand> allCommands = new HashSet<IGlobalCommand>();

        /// <summary>
        /// The command selector.
        /// </summary>
        private GlobalCommandSelection globalCommandSelection = GlobalCommandSelection.Default;

        /// <summary>
        /// Initializes a new instance of the <see cref="GlobalCommandManager"/> class.
        /// </summary>
        /// <param name="commands">
        /// The commands.
        /// </param>
        public GlobalCommandManager(Queue<IGlobalCommand> commands)
        {
            this.globalCommands = commands;
        }

        /// <summary>
        /// The create command.
        /// </summary>
        /// <param name="command">
        /// The command.
        /// </param>
        public void CreateCommand(object command)
        {
            if (command is IGlobalCommand { Command: { } } globalCommand)
            {
                this.globalCommands.Enqueue(globalCommand);
                return;
            }

            this.SetCommandOld(this.SetCommandByAttribute(command));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="selector"></param>
        public void CreateCommand(object instance, GlobalCommandSelection selector)
        {
            this.globalCommandSelection = selector;
            this.SetCommandOld(this.SetCommandByAttribute(instance));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="addAll"></param>
        public void CreateCommandByAttribute(object instance, bool addAll = false)
        {

            this.SetCommand(this.SetCommandByAttribute(instance), addAll);
        }

        /// <summary>
        /// Регистрирует команду кторую можно использовать в (моделе представлении). 
        /// Получить комманду можно с помощью метода <see cref="GlobalCommandManager.GetGlobalCommand"/> по ее имени после регистрации.
        /// </summary>
        /// <param name="name"> Имя глобальной команды. </param>
        /// <param name="command"> Ссылка на комнду. </param>
        /// <remarks>
        /// Имена каждой новой команды должны быть уникальными, иначе существующяя команда будет заменена новой командой. 
        /// Это бывает полезно когда ваша команда, создается каждый раз, при инициальзации конструктара и вам нужно обновить команду для нового объекта.
        /// </remarks>
        public void RegisterCommand(string name, ICommand command)
        {
            var globalCommand = new GlobalCommand
            {
                Name = name,
                Command = command,
                SourceFullName = command.GetType().FullName,
                SourceType = command.GetType()
            };

            if (this.allCommands.SingleOrDefault(c => c.Name == name) != null)
                this.allCommands.RemoveWhere(c => c.Name == name);
            this.allCommands.Add(globalCommand);
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
            GlobalCommand select = null;

            var selectionGlobalCommands = this.globalCommands.Where(c => ((GlobalCommand)c).Name.Contains(commandName));

            foreach (var globalCommand in selectionGlobalCommands)
            {
                if (globalCommand is not GlobalCommand command) continue;
                if (command.Delegate == null) return command;

                var baseType = command.Delegate.Method.GetBaseDefinition().DeclaringType;
                var type = command.SourceMethod.DeclaringType;
                select = command;

                //switch (globalCommand.SelectionFlag)
                //{
                //    case GlobalCommandSelection.All:

                //        break;
                //    case GlobalCommandSelection.SingleFirst:
                //        if (baseType != type) return command;
                //        select = command;
                //        continue;

                //    case GlobalCommandSelection.SingleLast:
                //        if (baseType != type) return command;
                //        select = command;
                //        continue;
                //}
            }
        
            return select;
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
        public IGlobalCommand GetGlobalCommand(string commandName) 
            => (GlobalCommand)this.allCommands.Single(cmd => ((GlobalCommand)cmd).Name.Contains(commandName));

        /// <summary>
        /// The get commands.
        /// </summary>
        /// <param name="commandName">
        /// The command name.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public IEnumerable<IGlobalCommand> GetGlobalCommands(string commandName) 
            => this.allCommands.Where(p => p.Name.Contains(commandName));

        /// <summary>
        /// The get commands.
        /// </summary>
        /// <returns>
        /// The <see cref="Queue"/>.
        /// </returns>
        public Queue<IGlobalCommand> GetCommands() => this.globalCommands;

        /// <summary>
        /// The set command.
        /// </summary>
        /// <param name="instance">
        /// The instance.
        /// </param>
        private IGlobalCommand SetCommandByAttribute(object instance)
        {
            const BindingFlags Flags = BindingFlags.Public | BindingFlags.Instance;
            Type type = instance.GetType();
            MethodInfo[] methods = type.GetMethods(Flags);

            foreach (var method in methods)
            {
                if (method.CustomAttributes.All(p => p.AttributeType.Name != "GlobalCommandAttribute")) continue;

                var action = Delegate.CreateDelegate(DelegateTypeFactory.Create(method), instance, method);
                var command = new GlobalCommandExecute(action, type);

                var globalCommand = new GlobalCommand
                { 
                    Command = command,
                    SourceFullName = instance.GetType().FullName,
                    SourceType = instance.GetType(),
                    SourceMethod = method
                };

                foreach (var attribute in method.GetCustomAttributes())
                {
                    var properties = attribute.GetType().GetProperties();
                    globalCommand.Name = (string)attribute.GetType().GetProperty("Name")?.GetValue(attribute);
                    globalCommand.ShortcutKey = (InputGesture)attribute.GetType().GetProperty("Hotkey")?.GetValue(attribute);
                }

                 return globalCommand;
            }

            return null;
        }

        [Obsolete]
        private void SetCommandOld(IGlobalCommand globalCommand) 
        {
            this.globalCommands.Enqueue(globalCommand);
        }

        private void SetCommand(IGlobalCommand globalCommand, bool addAll)
        {
            if (addAll)
            {
                this.allCommands.Add(globalCommand);
                return;
            }

            if (this.allCommands.SingleOrDefault(c => c.Name == globalCommand.Name) != null)
                this.allCommands.RemoveWhere(c => c.Name == globalCommand.Name);
            this.allCommands.Add(globalCommand);
        }

        /// <summary>
        /// The set command.
        /// </summary>
        /// <param name="instance">
        /// The instance.
        /// </param>
        private void SetCommand(object instance)
        {
            const BindingFlags Flags = BindingFlags.Public | BindingFlags.Instance;
            Type type = instance.GetType();
            MethodInfo[] methods = type.GetMethods(Flags);
            
            foreach (var method in methods)
            {
                if (method.CustomAttributes.All(p => p.AttributeType.Name != "GlobalCommandAttribute")) continue;
             
                var action = Delegate.CreateDelegate(DelegateTypeFactory.Create(method), instance, method);
                var command = new GlobalCommandExecute(action, type);

                var globalCommand = new GlobalCommand
                {
                    Command = command,
                    SourceFullName = instance.GetType().FullName,
                    SourceType = instance.GetType(),
                    SourceMethod = method
                };

                foreach (var attribute in method.GetCustomAttributes())
                {
                     var properties = attribute.GetType().GetProperties();
                     globalCommand.Name = (string)attribute.GetType().GetProperty("Name")?.GetValue(attribute);
                     globalCommand.ShortcutKey = (InputGesture)attribute.GetType().GetProperty("Hotkey")?.GetValue(attribute);
                }

                this.globalCommands.Enqueue(globalCommand);
            }
        }

        /// <summary>
        /// The update command.
        /// </summary>
        /// <param name="commandName">
        /// The command name.
        /// </param>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public void UpdateCommand(string commandName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The create singleton command.
        /// </summary>
        /// <param name="commandName">
        /// The command name.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
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
                BaseType = action.Method.DeclaringType,
                SourceFullName = action.Method.DeclaringType?.FullName,
                SourceMethod = action.Method,
                SourceType = action.Method.DeclaringType,
                Name = action.Method.Name,
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

        public IEnumerable<IGlobalCommand> GetCommands(string commandName)
        {
            throw new NotImplementedException();
        }
    }
}
