
using UnityCommander.Abstractions.Keyboard;
using UnityCommander.Common.Commands;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Services
{
    public sealed class ShortcutBinder : IShortcutBinder
    {
        private readonly IShortcutRegistry _shortcutRegistry;

        public ShortcutBinder(
            IShortcutRegistry shortcutRegistry,
            CommandPresentationProvider presentation)
        {
            _shortcutRegistry = shortcutRegistry;
        }

        public void Bind(
            string commandId,
            ShortcutKey key,
            ShortcutModifiers modifiers = ShortcutModifiers.None,
            ShortcutScope scopes = ShortcutScope.FilePanel | ShortcutScope.MainWindow)
        {
            _shortcutRegistry.Register(new ShortcutDefinition
            {
                CommandId = commandId,
                Description = CommandPresentationProvider.Get(commandId).Description,
                Key = key,
                Modifiers = modifiers,
                Scopes = scopes
            });
        }
    }
}
