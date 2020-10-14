
namespace UnityCommander.Modules.FilePanel.Commands
{
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
    }
}
