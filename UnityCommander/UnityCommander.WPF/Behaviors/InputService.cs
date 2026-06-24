
using System.Windows.Input;
using UnityCommander.Abstractions.Keyboard;
using UnityCommander.Core.Commands;
using UnityCommander.Core.Keyboad;

namespace UnityCommander.WPF.Behaviors
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

        public void Process(KeyEventArgs e)
        {
            var (key, mods) =
                WpfShortcutConverter.FromKeyGesture(e.Key, Keyboard.Modifiers);

            if (!ShortcutKeyValidator.IsValid(e.Key))
                return;

            if (!_resolver.TryResolve(key, mods, _context.Current, out var commandId))
                return;

            _executer.Execute(commandId);

            e.Handled = true;
        }
    }
}
