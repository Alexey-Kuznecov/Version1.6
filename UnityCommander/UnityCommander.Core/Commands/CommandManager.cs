
namespace UnityCommander.Core.Commands
{
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
        private readonly List<InvokerBase> commandCollection = new ();

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
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="NavigationInvoker"/>.
        /// </returns>
        public InvokerBase GetCommand(int index) 
            => this.commandCollection[index];

        /// <summary>
        /// The get command.
        /// </summary>
        /// <typeparam name="TInvokerBase">
        /// Type of command.
        /// </typeparam>
        /// <returns>
        /// The <see cref="InvokerBase"/>.
        /// </returns>
        public IEnumerable<InvokerBase> GetCommand<TInvokerBase>() 
            => this.commandCollection.Where(command => command is TInvokerBase);

        /// <summary>
        /// The set navigation command.
        /// </summary>
        /// <param name="register">
        /// The navigation invoker.
        /// </param>
        /// <returns>
        /// The <see cref="InvokerBase"/>.
        /// </returns>
        public InvokerBase CommandRegister(InvokerBase register)
        {
            this.commandCollection.Add(register);
            return register;
        } 
    }
}
