using System;
using System.Diagnostics;
using System.Windows.Input;

namespace AlexLibWpf.Mvvm.Base
{
    [DebuggerStepThrough]
    public class RelayCommand : ICommand
    {
        private readonly Action _command;
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;
        public event EventHandler CanExecuteChanged;

        public RelayCommand(Action command, Func<bool> canExecute = null)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));
            _command = command;
        }
        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            if (execute == null)
                throw new ArgumentNullException(nameof(execute));
            _execute = execute;
            _canExecute = canExecute;
        }
        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }
        public void Execute(object parameter)
        {
            _execute.Invoke(parameter);
        }
        public void Execute()
        {
            _command.Invoke();
        }
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
