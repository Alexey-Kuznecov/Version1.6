
namespace UnityCommander.Common.Invokers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using UnityCommander.Core.Commands;

    /// <summary>
    /// The procedures.
    /// </summary>
    public enum Scopes
    {
        /// <summary>
        /// The Macros.
        /// </summary>
        Macros,

        /// <summary>
        /// The module.
        /// </summary>
        Module,

        /// <summary>
        /// The all.
        /// </summary>
        All
    }

    /// <summary>
    /// This class is part of the implementation of the Command design pattern.
    /// Asks the command to carry out the request.
    /// </summary>
    public abstract class InvokerBase
    {
        /// <summary>
        /// The macros commands.
        /// </summary>
        protected static readonly List<Command> ModuleCommands = new List<Command>();

        /// <summary>
        /// The current command.
        /// </summary>
        protected static Command currentCommand;

        /// <summary>
        /// The current command.
        /// </summary>
        private static int index;

        /// <summary>
        /// Gets or sets the index.
        /// </summary>
        private static int Index 
        {   
            get => index;
            set => index = value < 0 ? 0 : value;
        }

        /// <summary>
        /// Executes the module commands in turn.
        /// </summary>
        public void Execute()
        {
            foreach (var cmd in ModuleCommands)
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
            return ModuleCommands;
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
            return ModuleCommands;
        }

        /// <summary>
        /// Enumerates module commands that can delegates
        /// the execution of the command to its methods.
        /// </summary>
        /// <param name="action"> The method to be called. </param>
        /// <param name="arg"> The string argument to be passed. </param>
        public virtual void Execute(Action<object> action, object arg)
        {
            foreach (var cmd in ModuleCommands.Where(cmd => cmd.CanExecute()))
            {
                cmd.Execute(action, arg);
            }

            Index++;
        }

        /// <summary>
        /// Cancels the changes that were made by execution command. For example,
        /// the command that can restore object or UI components previous state
        /// </summary>
        /// <param name="action"> Determines the method of the caller. </param>
        /// <param name="arg"> Determines the argument of the invoker. </param>
        public virtual void UnExecute(Action<object> action, object arg)
        {
            if (ModuleCommands.Count > 0)
            {
                ModuleCommands[index].UnExecute(action, ModuleCommands[index]);
            }

            Index--;
        }

        /// <summary>
        /// Adding a command to record macros.
        /// </summary>
        /// <param name="newCommand">
        /// The new command.
        /// </param>
        protected void AddMacros(Command newCommand)
        {
            ModuleCommands.Add(newCommand);
        }
    }
}
