
namespace UnityCommander.Core.Commands
{
    using System;
    using System.Collections.Generic;

    using UnityCommander.Core.Commands.Base;

    /// <summary>
    /// The navigation invoker.
    /// </summary>
    public sealed class NavigationInvoker : InvokerBase
    {
        /// <summary>
        /// The module commands. 
        /// </summary>
        private readonly List<Command> commands;

        /// <summary>
        /// The current command.
        /// </summary>
        private static Command currentCommand;

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationInvoker"/> class.
        /// </summary>
        public NavigationInvoker()
        {
            this.commands = new List<Command>();
        }

        /// <summary>
        /// Gets a value indicating whether the command can be canceled.
        /// </summary>
        public bool CanUndo => this.commands.Count != 1;

        /// <summary>
        /// Gets a value indicating whether the command can be repeated.
        /// </summary>
        public bool CanRedo => this.CurrentIndex > 0 && this.CurrentIndex < this.CurrentIndex - 1;

        /// <summary>
        /// The current index.
        /// </summary>
        public int CurrentIndex => this.commands.Count - 1;

        /// <summary>
        /// Adds a new command to the command execution history.
        /// </summary>
        /// <param name="action"> Allows defining the navigation command. </param>
        /// <param name="path"> Allows defining the command arguments. </param>
        public void AddCommand(Action<object> action, object path)
        {
            NavigationInvoker.currentCommand = new ConcreteCommand(new Navigator(action, path));
            this.commands.Add(NavigationInvoker.currentCommand);
        }


        /// <summary>
        /// Adds a new command without argument to the command execution history.
        /// </summary>
        /// <param name="action"> Allows defining the navigation command. </param>
        public void AddCommand(Action action)
        {
            NavigationInvoker.currentCommand = new ConcreteCommand(new Navigator(action));
            this.commands.Add(NavigationInvoker.currentCommand);
        }

        /// <summary>
        /// Execute new command no arguments.
        /// </summary>
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
            NavigationInvoker.currentCommand = new ConcreteCommand(new Navigator());            
            this.commands.Add(NavigationInvoker.currentCommand);
            this.commands[this.CurrentIndex]?.Execute(action, path);
        }

        /// <summary>
        /// Goes to the next directory if possible.
        /// </summary>
        public void Next()
        {
            if (this.CanRedo)
            {
                var command = this.commands[this.CurrentIndex];
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
                this.commands.RemoveAt(this.CurrentIndex);
                NavigationInvoker.currentCommand = this.commands[this.CurrentIndex];
                var receiver = (Navigator)((ConcreteCommand)NavigationInvoker.currentCommand)?.Receiver;
                
                if (receiver != null)
                {
                    if (receiver.CommandArg is not null)
                        currentCommand.UnExecute(receiver.CommandArg, receiver.Path);
                    else
                        currentCommand.UnExecute(receiver.Command);
                }
            }
        }
    }
}
