
namespace UnityCommander.Core.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using UnityCommander.Core.Commands.Base;

    /// <summary>
    /// The command manager.
    /// </summary>
    public class CommandManager
    {
        /// <summary>
        /// The navigation invoker.
        /// </summary>
        private readonly NavigationInvoker navigationInvoker;

        /// <summary>
        /// The navigation collection.
        /// </summary>
        private readonly Dictionary<Guid, InvokerBase> commandCollection = new ();

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandManager"/> class.
        /// </summary>
        public CommandManager()
        {
            this.navigationInvoker = new NavigationInvoker();
        }

        /// <summary>
        /// The get navigation command.
        /// </summary>
        /// <returns>
        /// The <see cref="NavigationInvoker"/>.
        /// </returns>
        public NavigationInvoker GetNavigationCommand() => this.navigationInvoker;

        /// <summary>
        /// The get navigation command.
        /// </summary>
        /// <param name="token">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="NavigationInvoker"/>.
        /// </returns>
        public InvokerBase GetCommand(Guid token) 
            => this.commandCollection.Single(cmd => cmd.Key.Equals(token)).Value;

        /// <summary>
        /// The set navigation command.
        /// </summary>
        /// <param name="token">
        /// The token.
        /// </param>
        /// <param name="register">
        /// The navigation invoker.
        /// </param>
        /// <returns>
        /// The <see cref="InvokerBase"/>.
        /// </returns>
        public InvokerBase CommandRegister(Guid token, InvokerBase register)
        {
            this.commandCollection.Add(token, register);
            return register;
        } 
    }
}
