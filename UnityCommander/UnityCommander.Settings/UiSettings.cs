
using UnityCommander.Settings.Core;

namespace UnityCommander.Settings
{
    public static class UiSettings
    {
        public static readonly SettingDefinition<string> SidebarPosition =
            new SettingDefinition<string>()
            {
                Key = "ui.sidebar.position",
                ValueType = typeof(string),
                DefaultValue = "left",
                Category = "UI"
            };
    }
}
