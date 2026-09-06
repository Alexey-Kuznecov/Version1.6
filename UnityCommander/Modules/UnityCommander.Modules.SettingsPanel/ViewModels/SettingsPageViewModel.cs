
using System.Collections.ObjectModel;

namespace UnityCommander.Modules.SettingsPanel.ViewModels
{
    public sealed class SettingsPageViewModel
    {
        public string IconKey { get; init; } = string.Empty;

        public string Title { get; init; } = string.Empty;

        public ObservableCollection<SettingItemViewModel> Items { get; }
            = new();
    }
}
