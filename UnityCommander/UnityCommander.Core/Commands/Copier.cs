
namespace UnityCommander.Core.Commands
{
    using System;

    using UnityCommander.Core.Commands.Base;

    /// <summary>
    /// The copier.
    /// </summary>
    public class Copier : ReceiverBase
    {
        /// <summary>
        /// Gets the path.
        /// </summary>
        public string Path { get; private set; }

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
        /// Update file panel when to a navigate directory.
        /// </summary>
        /// <param name="action"> The method for update file panel. </param>
        /// <param name="path"> Expected path to directory. </param>
        public override void Execute(Action<object> action, object path)
        {
            this.Path = path as string;
            action(path);
        }

        public override void Execute(Action action)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Cancels the changes that were made by execute command. For example,
        /// the command that can restore object or UI components previous state
        /// </summary>
        /// <param name="action"> Determines the method of the caller. </param>
        /// <param name="arg"> Expected path to directory. </param>
        public override void UnExecute(Action<object> action, object arg)
        {
            action(arg);
        }

        public override void UnExecute(Action action)
        {
            throw new NotImplementedException();
        }
    }
}
