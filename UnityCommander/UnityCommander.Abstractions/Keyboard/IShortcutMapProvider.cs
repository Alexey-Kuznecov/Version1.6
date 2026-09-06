
namespace UnityCommander.Abstractions.Keyboard
{
    public interface IShortcutMapProvider
    {
        bool TryGet(
            ShortcutGesture gesture,
            out ShortcutDefinition? shortcut);

        void Rebuild();
    }
}
