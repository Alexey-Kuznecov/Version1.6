
namespace UnityCommander.Core.Commands
{
    using System;
    using System.Collections.Generic;

    using UnityCommander.Core.Commands.Base;

    /// <summary>
    /// The copy file invoker.
    /// </summary>
    public class CopyFileInvoker
    {
        /// <summary>
        /// The module Commands.
        /// </summary>
        private static readonly List<Command> Commands = new List<Command>();

        /// <summary>
        /// Initializes a new instance of the <see cref="CopyFileInvoker"/> class.
        /// </summary>
        public CopyFileInvoker()
            : base()
        {
        }

        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="action"> The command to copy. </param>
        /// <param name="arg"> The anonymous object which contains the source and the target to copy. </param>
        public void Execute(Action<object> action, object arg)
        {
            Command command = new ConcreteCommand(new FileCopierReceiver());
            Commands.Add(command);
            command.Execute(action, arg);
        }

        /// <summary>
        /// Adding a command to record macros.
        /// </summary>
        private void AddCommand()
        {
            Commands.Add(new ConcreteCommand(new FileCopierReceiver()));
        }
    }
}
