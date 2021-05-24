
namespace UnityCommander.Core.Commands.Base
{
    using System;

    /// <summary>
    /// This class is part of the implementation of the Command design pattern.
    /// Knows how to perform the operations associated with carrying out the request.
    /// </summary>
    public abstract class ReceiverBase
    {
        /// <summary>
        /// Gets or sets the execute key.
        /// </summary>
        public string ExecuteKeys { get; set; }

        /// <summary>
        /// Determines can whether the command be executed.
        /// </summary>
        /// <returns> True if the command can be executed. </returns>
        public abstract bool CanExecute();

        /// <summary>
        /// Determines can whether the command to be executed.
        /// </summary>
        /// <param name="action"> Allows determined the method of the invoker. </param>
        /// <returns>
        /// Returns <see langword="true"/> if the command is valid.
        /// </returns>
        public abstract bool CanExecute(Action action);

        /// <summary>
        /// Execute new command no arguments.
        /// </summary>
        public abstract void Execute();

        /// <summary>
        /// Delegates execution command to invoker with the string parameter.
        /// </summary>
        /// <param name="action"> Determines the method of the invoker. </param>
        /// <param name="path"> Determines the method invoker argument. </param>
        public abstract void Execute(Action<object> action, object path);

        /// <summary>
        /// Cancels the changes that were made by <see cref="Execute"/> command. For example,
        /// the command that can restore object or UI components previous state
        /// </summary>
        /// <param name="action"> Determines the method of the caller. </param>
        /// <param name="arg"> Determines the method invoker argument. </param>
        public abstract void UnExecute(Action<object> action, object arg);
    }
}
