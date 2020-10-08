
namespace UnityCommander.Modules.FilePanel.Commands
{
    using System;
    using System.Collections.Generic;

    using UnityCommander.Common.Invokers;
    using UnityCommander.Core.Commands;

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
        /// The current index or path.
        /// </summary>
        private int currentIndex;

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
        public bool CanUndo => this.commands.Count > 0 && this.currentIndex > -1;

        /// <summary>
        /// Gets a value indicating whether the command can be repeated.
        /// </summary>
        public bool CanRedo => this.commands.Count > 0 && this.currentIndex < this.commands.Count - 1;

        /// <summary>
        /// The index.
        /// </summary>
        public int Index => this.currentIndex == 0 ? 0 : this.currentIndex - 1;

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
            this.currentIndex++;
        }

        /// <summary>
        /// Cancels the changes that were made by execution command. For example,
        /// the command that can restore object or UI components previous state
        /// </summary>
        /// <param name="action"> Allows defining the navigation command. </param>
        /// <param name="path"> Allows defining the command arguments. </param>
        public override void Execute(Action<object> action, object path)
        {
            NavigationInvoker.currentCommand = new ConcreteCommand(new Navigator());            
            this.commands.Add(NavigationInvoker.currentCommand);
            this.commands[this.currentIndex++]?.Execute(action, path);
        }

        /// <summary>
        /// Goes to the next directory if possible.
        /// </summary>
        public void Next()
        {
            if (this.CanRedo)
            {
                var command = this.commands[this.currentIndex];
                var receiver = (Navigator)((ConcreteCommand)command)?.Receiver;
                command?.Execute(receiver.Action, receiver.Path);
            }
        }

        /// <summary>
        /// Goes to the previous directory if possible.
        /// </summary>
        public void Previous()
        {
            if (this.CanUndo)
            {
                this.currentIndex = this.commands.Count - 1;

                if (this.commands.Count == 1) return;
                this.commands.RemoveAt(this.currentIndex);
                NavigationInvoker.currentCommand = this.commands[this.Index];
                var receiver = (Navigator)((ConcreteCommand)NavigationInvoker.currentCommand)?.Receiver;
                
                if (receiver != null)
                {
                    currentCommand.UnExecute(receiver.Action, receiver.Path);
                }
            }
        }
    }
}
