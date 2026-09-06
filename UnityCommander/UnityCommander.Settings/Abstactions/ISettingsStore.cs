
using UnityCommander.Settings.Core;

namespace UnityCommander.Settings.Abstactions
{
    public interface ISettingsStore
    {
        Dictionary<string, object?> Load(Dictionary<string, SettingDefinition> definitions);

        void Save(Dictionary<string, object?> values);
    }
}
