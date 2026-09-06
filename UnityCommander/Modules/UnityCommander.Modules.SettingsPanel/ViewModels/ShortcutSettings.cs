
using System.Collections.Generic;
using UnityCommander.Abstractions.Keyboard;

namespace UnityCommander.Modules.SettingsPanel.ViewModels
{
    public sealed class ShortcutSettings
    {
        public List<ShortcutDefinition> Items { get; set; } = [];
    }
}
