
using System.Windows.Input;

namespace UnityCommander.UI.Interaction
{
    public sealed class ContextActionCommand : ICommand
    {
        private readonly IContextAction _action;
        private readonly IInteractionContext _context;

        public ContextActionCommand(
            IContextAction action,
            IInteractionContext context)
        {
            _action = action;
            _context = context;
        }

        public bool CanExecute(object? parameter)
            => _action.CanExecute(_context);

        public void Execute(object? parameter)
            => _ = _action.ExecuteAsync(_context);

        public event EventHandler? CanExecuteChanged;
    }
}
