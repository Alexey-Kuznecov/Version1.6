
namespace UnityCommander.Abstractions.Keyboard
{
    public interface IShortcutRegistry
    {
        IReadOnlyCollection<ShortcutDefinition> GetAll();

        void Register(ShortcutDefinition definition);

        bool TryGet(string commandId, out ShortcutDefinition definition);

        bool Remove(string commandId);

        void Clear();
    }
}
