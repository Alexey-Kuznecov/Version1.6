
namespace UnityCommander.Settings.Abstactions
{
    public interface ISettingsStore
    {
        Dictionary<string, object?> Load();

        void Save(Dictionary<string, object?> values);
    }
}
