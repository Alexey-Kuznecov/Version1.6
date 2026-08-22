using UnityCommander.Abstractions.Keyboard;
using UnityCommander.Core.Commands;
using UnityCommander.Core.Keyboad;

namespace UnityCommander.WPF.Input
{
    public class InputService : IInputService
    {
        private readonly IShortcutContextService _context;
        private IShortcutResolver _resolver;
        private ICommandExecuter _executer;

        public InputService(
            IShortcutContextService shortcutContext, 
            IShortcutResolver resolver,
            ICommandExecuter executer)
        {
            _context = shortcutContext;
            _resolver = resolver;
            _executer = executer;
        }

        public bool Process(InputEvent input)
        {
            if (!ShortcutKeyValidator.IsValid(input.Key, input.Modifiers))
                return false;

            if (!_resolver.TryResolve(input.Key, input.Modifiers, _context.Current, out var commandId))
                return false;

            _executer.Execute(commandId);

            return true;
        }
    }
}
