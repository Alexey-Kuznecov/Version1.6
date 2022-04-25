
using System;
using System.Diagnostics;
using System.Windows.Input;

namespace AlexeyKuznecov.Library.Mvvm.Base
{
    /// <summary>
    /// TODO The relay command.
    /// </summary>
    [DebuggerStepThrough]
    public class RelayCommand : ICommand
    {
        /// <summary>
        /// The command.
        /// </summary>
        private readonly Action command;

        /// <summary>
        /// The execute.
        /// </summary>
        private readonly Action<object> execute;

        /// <summary>
        /// The can execute.
        /// </summary>
        private readonly Predicate<object> canExecute;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand"/> class.
        /// </summary>
        /// <param name="command">
        /// The command.
        /// </param>
        /// <param name="canExecute">
        /// The can execute.
        /// </param>
        public RelayCommand(Action command, Func<bool> canExecute = null)
        {
            this.command = command ?? throw new ArgumentNullException(nameof(command));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand"/> class.
        /// </summary>
        /// <param name="execute">
        /// The execute.
        /// </param>
        /// <param name="canExecute">
        /// The can execute.
        /// </param>
        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
            this.canExecute = canExecute;
        }

        /// <summary>
        /// The can execute changed.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// The can execute.
        /// </summary>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool CanExecute(object parameter)
        {
            return this.canExecute == null || this.canExecute(parameter);
        }

        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        public void Execute(object parameter)
        {
            this.execute?.Invoke(parameter);
        }

        /// <summary>
        /// The execute.
        /// </summary>
        public void Execute()
        {
            this.command.Invoke();
        }

        /// <summary>
        /// The raise can execute changed.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
