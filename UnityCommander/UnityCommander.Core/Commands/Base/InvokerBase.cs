
namespace UnityCommander.Core.Commands.Base
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

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
    [SuppressMessage("ReSharper", "StyleCop.SA1503")]
    public abstract class InvokerBase
    {
        /// <summary>
        /// Commands that will be added and available as global commands.
        /// </summary>
        protected static readonly List<Command> MacrosCommands = new ();
        
        /// <summary>
        /// The current command index.
        /// </summary>
        private static int index;

        /// <summary>
        /// The execute changed.
        /// </summary>
        /// <param name="argument">
        /// The argument.
        /// </param>
        public delegate void ExecuteChanged(object argument);

        /// <summary>
        /// Gets or sets the can execute changed.
        /// </summary>
        public event ExecuteChanged OnExecuteChanged;

        /// <summary>
        /// Gets or sets commands that will be added and available only for a current module.
        /// </summary>
        protected List<Command> ModuleCommands { get; set; }

        /// <summary>
        /// Gets or sets the index.
        /// </summary>
        private static int Index 
        {   
            get => index;
            set => index = value < 0 ? 0 : value;
        }

        /// <summary>
        /// The raise execute changed.
        /// </summary>
        /// <param name="argument">
        /// The argument.
        /// </param>
        public void RaiseExecuteChanged(object argument)
        {
            this.OnExecuteChanged?.Invoke(argument);
        }

        /// <summary>
        /// Gets a commands of modules in turn for the function invoker.
        /// To enumerate commands, you need to use a loop.
        /// </summary>
        /// <returns>
        /// Returns enumerator of the commands all modules.
        /// </returns>
        public IEnumerable<Command> GetAllCommands() => MacrosCommands;

        /// <summary>
        /// Gets the module commands in turn for the function invoker.
        /// To enumerate commands, you need to use a loop.
        /// </summary>
        /// <returns>
        /// Returns enumerator to get each command of the module.   .
        /// </returns>
        public IEnumerable<Command> GetCommands() => this.ModuleCommands;
        
        /// <summary>
        /// Enumerates module commands that can delegates
        /// the execution of the command to its methods.
        /// </summary>
        /// <param name="action"> The method to be called. </param>
        public virtual void Execute(Action action)
        {
            foreach (var cmd in this.ModuleCommands)
            {
                if (!cmd.CanExecute()) continue;
                cmd.Execute(action);
            }

            Index++;
        }

        /// <summary>
        /// Enumerates module commands that can delegates
        /// the execution of the command to its methods.
        /// </summary>
        /// <param name="action"> The method to be called. </param>
        /// <param name="arg"> The string argument to be passed. </param>
        public virtual void Execute(Action<object> action, object arg)
        {
            foreach (var cmd in this.ModuleCommands.Where(cmd => cmd.CanExecute()))
            {
                cmd.Execute(action, arg);
            }

            Index++;
        }

        /// <summary>
        /// Cancels the changes that were made by execution command.
        /// </summary>
        /// <param name="action"> Determines the method of the caller. </param>
        /// <param name="arg"> Determines the argument of the invoker. </param>
        public virtual void UnExecute(Action<object> action, object arg)
        {
            if (this.ModuleCommands.Count > 0)
            {
                this.ModuleCommands[index].UnExecute(action, this.ModuleCommands[index]);
            }

            Index--;
        }

        /// <summary>
        /// Cancels the changes that were made by execution command.
        /// </summary>
        /// <param name="action"> Determines the method of the caller. </param>
        public virtual void UnExecute(Action action)
        {
            if (this.ModuleCommands.Count > 0)
            {
                this.ModuleCommands[index].UnExecute(action);
            }

            Index--;
        }
        
        /// <summary>
        /// The remove.
        /// </summary>
        /// <param name="command">
        /// The command.
        /// </param>
        public void Remove(Command command)
        {
            if (this.ModuleCommands.Contains(command))
            {
                this.ModuleCommands.Remove(command);
            }

            if (MacrosCommands.Contains(command))
            {
                MacrosCommands.Remove(command);
            }
        }

        /// <summary>
        /// The remove.
        /// </summary>
        public virtual void RemoveAll()
        {
            this.ModuleCommands.Clear();
            MacrosCommands.Clear();
        }

        /// <summary>
        /// Adding a command to record macros.
        /// </summary>
        /// <param name="newCommand">
        /// The new command.
        /// </param>
        protected void AddMacros(Command newCommand) => MacrosCommands.Add(newCommand);
    }
}
