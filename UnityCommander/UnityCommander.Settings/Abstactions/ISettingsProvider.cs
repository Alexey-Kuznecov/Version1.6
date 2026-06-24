
using UnityCommander.Settings.Core;

namespace UnityCommander.Abstractions.Settings
{
    public interface ISettingsProvider
    {
        IEnumerable<SettingDefinition> GetDefinitions();
    }
}
