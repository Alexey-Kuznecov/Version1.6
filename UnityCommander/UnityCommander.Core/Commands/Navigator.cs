
namespace UnityCommander.Core.Commands
{
    using System;

    using UnityCommander.Core.Commands.Base;

    /// <summary>
    /// The navigator.
    /// </summary>
    public class Navigator : ReceiverBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Navigator"/> class.
        /// </summary>
        public Navigator()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Navigator"/> class.
        /// This constructor allows pre-registering the command.
        /// </summary>
        /// <param name="action"> Allows determined the method to get. </param>
        public Navigator(Action action)
        {
            this.Command = action;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Navigator"/> class.
        /// This constructor allows pre-registering the command.
        /// </summary>
        /// <param name="action"> Allows determined the method to get. </param>
        /// <param name="path"> Allows determined the path to directory. </param>
        public Navigator(Action<object> action, object path)
        {
            this.Path = path as string;
            this.CommandArg = action;
        }

        /// <summary>
        /// Gets the path.
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// Gets an action with an argument.
        /// </summary>
        public Action<object> CommandArg { get; private set; }

        /// <summary>
        /// Gets an action with no arguments.
        /// </summary>
        public Action Command { get; private set; }

        /// <summary>
        /// Determines can whether the command be executed.
        /// </summary>
        /// <returns> True if the command can be executed. </returns>
        public override bool CanExecute()
        {
            return true;
        }

        /// <summary>
        /// Determines can whether the command to be executed.
        /// </summary>
        /// <param name="action"> Allows determined the method of the invoker. </param>
        /// <returns>
        /// Returns <see langword="true"/> if the command is valid.
        /// </returns>
        public override bool CanExecute(Action action)
        {
            return true;
        }

        /// <summary>
        /// Execute new command no arguments.
        /// </summary>
        /// <param name="action">
        /// The action.
        /// </param>
        public override void Execute(Action action)
        {
            this.Command = action;
            action();
        }

        /// <summary>
        /// Cancels the changes that were made by execute command.
        /// </summary>
        /// <param name="action"> Determines the method of the caller. </param>
        public override void UnExecute(Action action)
        {
            action();
        }

        /// <summary>
        /// Update file panel when to a navigate directory.
        /// </summary>
        /// <param name="action"> The method for update file panel. </param>
        /// <param name="path"> Expected path to directory. </param>
        public override void Execute(Action<object> action, object path)
        {
            this.Path = path as string;
            this.CommandArg = action;
            action(path);
        }

        /// <summary>
        /// Cancels the changes that were made by execute command. For example,
        /// the command that can restore object or UI components previous state.
        /// </summary>
        /// <param name="action"> Determines the method of the caller. </param>
        /// <param name="arg"> Expected path to directory. </param>
        public override void UnExecute(Action<object> action, object arg)
        {
            action(arg);
        }
    }
}
