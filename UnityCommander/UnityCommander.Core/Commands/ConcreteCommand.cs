
namespace UnityCommander.Core.Commands
{
    using System;

    /// <summary>
    /// This class is part of the implementation of the Command design pattern.
    /// Defines a binding between a Receiver <c>object</c> and an action
    /// implements Execute by invoking the corresponding operation(s) on Receiver <see cref="ReceiverBase"/>.
    /// </summary>
    public class ConcreteCommand : Command
    {
        /// <summary>
        /// The id command.
        /// </summary>
        private static int idCommand;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcreteCommand"/> class.
        /// </summary>
        /// <param name="receiver">
        /// The Receiver.
        /// </param>
        public ConcreteCommand(ReceiverBase receiver)
        {
            idCommand = idCommand + 1;
            this.Id = idCommand;
            this.Receiver = receiver;
        }

        /// <summary>
        /// Gets the receiver instance.
        /// </summary>
        public ReceiverBase Receiver { get; }

        /// <summary>
        /// Cancels the changes that were made by <see cref="Execute"/> command. For example,
        /// the command that can restore object or UI components previous state
        /// </summary>
        /// <param name="action"> Determines the method of the caller. </param>
        /// <param name="arg"> Determines the method invoker argument. </param>
        public override void UnExecute(Action<object> action, object arg)
        {
            this.Receiver.UnExecute(action, arg);
        }

        /// <summary>
        /// Determines can whether the command be executed.
        /// </summary>
        /// <returns> True if the command can be executed. </returns>
        public override bool CanExecute()
        {
           return this.Receiver.CanExecute();
        }

        /// <summary>
        /// Allows the invoker to decide when the command will be executed.
        /// </summary>
        /// <param name="action">
        /// Allows defined the method of the invoker.
        /// </param>
        /// <returns>
        /// The command will be executed if the return value is <see langword="true"/>.
        /// </returns>
        public override bool CanExecute(Action action)
        {
           return this.Receiver.CanExecute(action);
        }

        /// <summary>
        /// Execute new command no arguments.
        /// </summary>
        public override void Execute()
        {
            this.Receiver.Execute();
        }

        /// <summary>
        /// Delegates execution command to invoker with the string parameter.
        /// </summary>
        /// <param name="action"> Determines the method of the caller. </param>
        /// <param name="arg"> Determines the method invoker argument. </param>
        public override void Execute(Action<object> action, object arg)
        {
            this.Receiver.Execute(action, arg);
        }
    }
}
