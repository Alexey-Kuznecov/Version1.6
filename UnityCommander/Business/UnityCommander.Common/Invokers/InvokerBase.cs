
namespace UnityCommander.Common.Invokers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using UnityCommander.Core.Commands;

    /// <summary>
    /// This class is part of the implementation of the Command design pattern.
    /// Asks the command to carry out the request.
    /// </summary>
    public class InvokerBase
    {
        /// <summary>
        /// The macros commands.
        /// </summary>
        protected static readonly List<Command> MacrosCommands = new List<Command>();

        /// <summary>
        /// The index.
        /// </summary>
        private static int index;
        
        /// <summary>
        /// The module commands.
        /// </summary>
        private readonly List<Command> moduleCommands = new List<Command>();

        /// <summary>
        /// Initializes a new instance of the <see cref="InvokerBase"/> class.
        /// </summary>
        /// <param name="command"> The <see cref="Command"/> class. </param>
        public InvokerBase(Command command)
        {
            this.moduleCommands.Add(command);
            MacrosCommands.Add(command);
        }

        /// <summary>
        /// Executes the module commands in turn.
        /// </summary>
        public void Execute()
        {
            foreach (var cmd in this.moduleCommands)
            {
                if (!cmd.CanExecute()) continue;
                cmd.Execute();
            }
        }

        /// <summary>
        /// Executes the commands all modules in turn.
        /// </summary>
        public void ExecuteAll()
        {
            foreach (var cmd in MacrosCommands)
            {
                if (!cmd.CanExecute()) continue;
                cmd.Execute();
            }
        }

        /// <summary>
        /// Gets a commands of modules in turn for the function invoker.
        /// To enumerate commands, you need to use a loop.
        /// </summary>
        /// <returns>
        /// Returns enumerator of the commands all modules.
        /// </returns>
        public IEnumerable<Command> GetAllCommands()
        {
            return MacrosCommands;
        }

        /// <summary>
        /// Gets the module commands in turn for the function invoker.
        /// To enumerate commands, you need to use a loop.
        /// </summary>
        /// <returns>
        /// Returns enumerator to get each command of the module.   .
        /// </returns>
        public IEnumerable<Command> GetCommands()
        {
            return this.moduleCommands;
        }

        /// <summary>
        /// Enumerates module commands that can delegates
        /// the execution of the command to its methods.
        /// </summary>
        /// <param name="action"> The method to be called. </param>
        /// <param name="arg"> The string argument to be passed. </param>
        public void Execute(Action<object> action, object arg)
        {
            foreach (var cmd in this.moduleCommands.Where(cmd => cmd.CanExecute()))
            {
                cmd.Execute(action, arg);
            }
        }

        /// <summary>
        /// Cancels the changes that were made by execution command. For example,
        /// the command that can restore object or UI components previous state
        /// </summary>
        /// <param name="action"> Determines the method of the caller. </param>
        /// <param name="arg"> Determines the argument of the invoker. </param>
        public virtual void UnExecute(Action<object> action, object arg)
        {
            if (this.moduleCommands.Count > 0)
            {
                foreach (var cmd in this.moduleCommands)
                {
                    cmd.UnExecute(action, this.moduleCommands[cmd.Id]);
                }
            }
        }
    }
}
