
namespace UnityCommander.Core.Commands
{
    using System;

    using UnityCommander.Core.Commands.Base;

    /// <summary>
    /// The copier.
    /// </summary>
    public class FileCopierReceiver : ReceiverBase
    {
        /// <summary>
        /// Gets or sets the path.
        /// </summary>
        public string Path { get; set; }

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
        public override void Execute()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Update file panel when to a navigate directory.
        /// </summary>
        /// <param name="action"> The method for update file panel. </param>
        /// <param name="path"> Expected path to directory. </param>
        public override void Execute(Action<object> action, object path)
        {
            action(path);
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
    }
}
