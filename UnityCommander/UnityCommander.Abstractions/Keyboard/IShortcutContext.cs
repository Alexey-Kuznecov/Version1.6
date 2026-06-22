
namespace UnityCommander.Abstractions.Keyboard
{
    public interface IShortcutContextService
    {
        ShortcutScope Current { get; }

        void Push(object owner, ShortcutScope scope);

        void Pop(object owner);
    }
}
