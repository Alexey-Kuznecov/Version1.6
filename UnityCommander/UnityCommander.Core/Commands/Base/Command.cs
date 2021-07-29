
namespace UnityCommander.Core.Commands.Base
{
    using System;

    /// <summary>
    /// This class is part of the implementation of the Command design pattern.
    /// Declares an abstract class for executing an operation.
    /// </summary>
    public abstract class Command
    {
        /// <summary>
        /// Gets or sets the command id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Executes the command no arguments.
        /// </summary>
        /// <param name="action">
        /// The action.
        /// </param>
        public abstract void Execute(Action action);

        /// <summary>
        /// Cancels the changes that were made by <see cref="Execute"/> command.
        /// </summary>
        /// <param name="action"> Determines the method of the caller. </param>
        public abstract void UnExecute(Action action);

        /// <summary>
        /// Delegates execution command to invoker with the <see langword="object"/> parameter.
        /// </summary>
        /// <param name="action"> Determines the method of the caller. </param>
        /// <param name="arg"> The argument of the invoker. </param>
        public abstract void Execute(Action<object> action, object arg);

        /// <summary>
        /// Cancels the changes that were made by <see cref="Execute"/> command. For example,
        /// the command that can restore object or UI components previous state
        /// </summary>
        /// <param name="action"> Determines the method of the caller. </param>
        /// <param name="arg"> Determines the argument of the invoker. </param>
        public abstract void UnExecute(Action<object> action, object arg);

        /// <summary>
        /// Determines can whether the command be executed.
        /// </summary>
        /// <returns> True if the command can be executed. </returns>
        public abstract bool CanExecute();

        /// <summary>
        /// Allows the invoker to decide when the command will be executed.
        /// </summary>
        /// <param name="action">
        /// Allows defined the method of the invoker.
        /// </param>
        /// <returns>
        /// The command will be executed if the return value is <see langword="true"/>.
        /// </returns>
        public abstract bool CanExecute(Action action);
    }
}
