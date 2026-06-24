
namespace UnityCommander.Abstractions.Keyboard
{
    public interface IShortcutOverrideStore
    {
        IReadOnlyCollection<ShortcutOverride> GetAll();

        public Dictionary<string, ShortcutOverride> GetSnapshot();

        void Set(ShortcutOverride value);

        bool TrySet(ShortcutOverride value);

        void Remove(string commandId);
    }
}
