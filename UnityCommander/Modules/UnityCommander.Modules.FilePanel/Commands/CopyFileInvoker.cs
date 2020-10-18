
namespace UnityCommander.Modules.FilePanel.Commands
{
    using System;
    using System.Collections.Generic;

    using UnityCommander.Common.Invokers;
    using UnityCommander.Core.Commands;

    /// <summary>
    /// The copy file invoker.
    /// </summary>
    public class CopyFileInvoker : InvokerBase
    {
        /// <summary>
        /// The module Commands.
        /// </summary>
        private static readonly List<Command> Commands = new List<Command>();

        /// <summary>
        /// Initializes a new instance of the <see cref="CopyFileInvoker"/> class.
        /// </summary>
        /// <param name="invoker">
        /// The invoker.
        /// </param>
        public CopyFileInvoker(CopyFileInvoker invoker)
            : base()
        {
        }

        /// <summary>
        /// Adding a command to record macros.
        /// </summary>
        private void AddCommand()
        {
            Commands.Add(new ConcreteCommand(new Copier()));
        }

        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="action"> The command to copy. </param>
        /// <param name="arg"> The anonymous object which contains the source and the target to copy. </param>
        public override void Execute(Action<object> action, object arg)
        {
            Command command = new ConcreteCommand(new Copier());
            Commands.Add(command);
            command.Execute(action, arg);
        }
    }
}
