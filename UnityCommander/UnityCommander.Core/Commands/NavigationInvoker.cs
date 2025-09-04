
namespace UnityCommander.Core.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using UnityCommander.Core.Commands.Base;

    /// <summary>
    /// The navigation invoker.
    /// </summary>
    [SuppressMessage("ReSharper", "StyleCop.SA1503")]
    public sealed class NavigationInvoker : InvokerBase
    {
        /// <summary>
        /// The current command.
        /// </summary>
        private static Command currentCommand;

        /// <summary>
        /// The navigator.
        /// </summary>
        private Navigator navigator;

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationInvoker"/> class.
        /// </summary>
        public NavigationInvoker()
        {
            this.ModuleCommands = new List<Command>();
        }

        /// <summary>
        /// Gets a value indicating whether the command can be canceled.
        /// </summary>
        public bool CanUndo => this.ModuleCommands.Count != 1;

        /// <summary>
        /// Gets a value indicating whether the command can be repeated.
        /// </summary>
        public bool CanRedo => this.CurrentIndex > 0 && this.CurrentIndex < this.CurrentIndex - 1;

        /// <summary>
        /// The next.
        /// </summary>
        public Command NextCommand => this.ModuleCommands.Count > 1 ? this.ModuleCommands[this.CurrentIndex + 1] : default(Command);

        /// <summary>
        /// The previous command.
        /// </summary>
        public Command PreviousCommand => this.ModuleCommands.Count > 0 ? this.ModuleCommands[this.CurrentIndex - 1] : default(Command);

        /// <summary>
        /// The current command.
        /// </summary>
        public Command CurrentCommand => this.ModuleCommands.Count != 0 ? this.ModuleCommands[this.CurrentIndex] : default(Command);

        /// <summary>
        /// The current command.
        /// </summary>
        public Command FirstCommand => this.ModuleCommands.Count != 0 ? this.ModuleCommands[0] : default(Command);
        
        /// <summary>
        /// The current index.
        /// </summary>
        public int CurrentIndex => this.ModuleCommands.Count - 1;

        /// <summary>
        /// Adds a new command to the command execution history.
        /// </summary>
        /// <param name="action"> Allows defining the navigation command. </param>
        /// <param name="path"> Allows defining the command arguments. </param>
        public void AddCommand(Action<object> action, object path)
        {
            NavigationInvoker.currentCommand = new ConcreteCommand(new Navigator(action, path));
            this.ModuleCommands.Add(NavigationInvoker.currentCommand);
        }

        /// <summary>
        /// Adds a new command without argument to the command execution history.
        /// </summary>
        /// <param name="action"> Allows defining the navigation command. </param>
        public void AddCommand(Action action)
        {
            NavigationInvoker.currentCommand = new ConcreteCommand(new Navigator(action));
            this.ModuleCommands.Add(NavigationInvoker.currentCommand);
        }

        public void AddCommand(Func<object, Task> asyncAction, object path)
        {
            //NavigationInvoker.currentCommand = new ConcreteCommandAsync(new AsyncNavigator(asyncAction, path));
            this.ModuleCommands.Add(NavigationInvoker.currentCommand);
        }

        /// <summary>
        /// The get command.
        /// </summary>
        /// <returns>
        /// The <see cref="Navigator"/>.
        /// </returns>
        public Command GetCommand()
        {
           return NavigationInvoker.currentCommand; 
        }

        /// <summary>
        /// Execute new command no arguments.
        /// </summary>
        /// <param name="action">
        /// The action.
        /// </param>
        public override void Execute(Action action)
        {
            action();
        }

        /// <summary>
        /// Delegates execution command to invoker with the string parameter.
        /// </summary>
        /// <param name="action"> Allows defining the navigation command. </param>
        /// <param name="path"> Allows defining the command arguments. </param>
        public override void Execute(Action<object> action, object path)
        {
            navigator = new Navigator(action, path);
            NavigationInvoker.currentCommand = new ConcreteCommand(navigator);
            this.ModuleCommands.Add(NavigationInvoker.currentCommand);
            this.ModuleCommands[this.CurrentIndex]?.Execute(action, path);
            this.RaiseExecuteChanged((ConcreteCommand)currentCommand);
        }

        /// <summary>
        /// Goes to the next directory if possible.
        /// </summary>
        public void Next()
        {
            if (this.CanRedo)
            {
                var command = this.ModuleCommands[this.CurrentIndex];
                var receiver = (Navigator)((ConcreteCommand)command)?.Receiver;
                command?.Execute(receiver.CommandArg, receiver.Path);
            }
        }

        /// <summary>
        /// Goes to the previous directory if possible.
        /// </summary>
        public void Previous()
        {
            if (this.CanUndo)
            {
                this.ModuleCommands.RemoveAt(this.CurrentIndex);
                NavigationInvoker.currentCommand = this.ModuleCommands[this.CurrentIndex];
                var receiver = (Navigator)((ConcreteCommand)NavigationInvoker.currentCommand)?.Receiver;
                
                if (receiver != null)
                {
                    if (receiver.CommandArg != null)
                        currentCommand.UnExecute(receiver.CommandArg, receiver.Path);
                    else
                        currentCommand.UnExecute(receiver.Command);
                }
            }
        }

        /// <summary>
        /// The remove all.
        /// </summary>
        public override void RemoveAll()
        {
            var count = this.ModuleCommands.Count - 1;

            for (int i = count; i >= 1; i--)
            {
                var item = this.ModuleCommands[i];
                this.ModuleCommands.Remove(item);
            }
        }
    }
}
