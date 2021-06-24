
// ReSharper disable All
namespace AkuzIcons.Mvvm.Base
{
    using System;
    using System.Diagnostics;
    using System.Windows.Input;

    ///[DebuggerStepThrough]
    public class RelayCommand : ICommand
    {
        private readonly Action _command;
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand"/> class.
        /// </summary>
        /// <param name="command">
        /// The command.
        /// </param>
        /// <param name="canExecute">
        /// The can execute.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        public RelayCommand(Action command, Func<bool> canExecute = null)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));
            _command = command;
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
        /// <exception cref="ArgumentNullException">
        /// </exception>
        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            if (execute == null)
                throw new ArgumentNullException(nameof(execute));
            _execute = execute;
            _canExecute = canExecute;
        }

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
            return _canExecute == null || _canExecute(parameter);
        }

        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        public void Execute(object parameter)
        {
            _execute.Invoke(parameter);
        }

        /// <summary>
        /// The execute.
        /// </summary>
        public void Execute()
        {
            _command.Invoke();
        }

        /// <summary>
        /// The raise can execute changed.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
