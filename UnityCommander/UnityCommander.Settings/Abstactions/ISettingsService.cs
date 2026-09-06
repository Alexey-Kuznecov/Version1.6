
using UnityCommander.Settings.Core;

namespace UnityCommander.Settings.Abstactions
{
     public interface ISettingsService
     {
        IEnumerable<SettingDefinition> GetDefinitions();

        IEnumerable<SettingDefinition<T>> GetDefinitions<T>();

        object Get(SettingDefinition def);

        T Get<T>(SettingDefinition<T> definition);
     
        void Reset<T>(SettingDefinition<T> definition);
        
        void Set<T>(SettingDefinition<T> definition, T value);

        void Set(SettingDefinition definition, object value);
    }
}
