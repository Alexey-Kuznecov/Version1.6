
using UnityCommander.Abstractions.Keyboard;

namespace UnityCommander.Services.Interfaces
{
    public interface IShortcutBinder
    {
        void Bind(
            string commandId,
            ShortcutKey key,
            ShortcutModifiers modifiers = ShortcutModifiers.None,
            ShortcutScope scopes = ShortcutScope.FilePanel | ShortcutScope.MainWindow);
    }
}
