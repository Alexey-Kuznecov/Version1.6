
namespace UnityCommander.Abstractions.Keyboard
{
    public interface IShortcutOverrideStore
    {
        IReadOnlyCollection<ShortcutOverride> GetAll();

        public Dictionary<string, ShortcutOverride> GetSnapshot();

        bool TryGet(string commandId, out ShortcutOverride value);

        void Set(ShortcutOverride value);

        bool TrySet(ShortcutOverride value);

        void Remove(string commandId);
    }
}
