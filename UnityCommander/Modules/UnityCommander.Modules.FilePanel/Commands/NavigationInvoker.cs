
namespace UnityCommander.Modules.FilePanel.Commands
{
    using System;

    using UnityCommander.Common.Invokers;
    using UnityCommander.Core.Commands;

    /// <summary>
    /// The navigation invoker.
    /// </summary>
    public class NavigationInvoker : InvokerBase
    {
        /// <summary>
        /// The command.
        /// </summary>
        private static Command command;

        /// <summary>
        /// Initializes static members of the <see cref="NavigationInvoker"/> class.
        /// </summary>
        static NavigationInvoker()
        {
            command = new ConcreteCommand(new Navigator());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationInvoker"/> class.
        /// </summary>
        public NavigationInvoker()
            : base(command)
        {
        }

        /// <summary>
        /// Adding a command to record macros.
        /// </summary>
        private static void AddMactos()
        {
            command = new ConcreteCommand(new Navigator());
            MacrosCommands.Add(command);
        }
    }
}
