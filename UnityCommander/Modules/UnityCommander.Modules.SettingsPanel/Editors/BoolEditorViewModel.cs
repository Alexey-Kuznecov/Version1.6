
using UnityCommander.Modules.SettingsPanel.Models;
using UnityCommander.Settings.Abstactions;

namespace UnityCommander.Modules.SettingsPanel.Editors
{
    public sealed class BoolEditorViewModel
    {
        private ISettingsService _settingsService;

        public BoolEditorViewModel(
            ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public SettingEntry<bool> Entry { get; set; }

        public string DisplayName => Entry.Definition.DisplayName;

        public string Description => Entry.Definition.Description;

        private bool _value;
      
        public bool Value
        {
            get => _value;
            set
            {
                if (_value == value)
                    return;

                _value = value;
                _settingsService.Set(Entry.Definition, value);
            }
        }
    }
}
