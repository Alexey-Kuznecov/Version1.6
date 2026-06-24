
using Prism.Mvvm;
using System.Windows;
using UnityCommander.Settings.Core;

namespace UnityCommander.Modules.SettingsPanel.ViewModels
{
    public class SettingItemViewModel : BindableBase
    {
        public SettingDefinition Definition { get; init; }
        public string Title { get; set; }
        public string Description { get; set; }
        public FrameworkElement Editor { get; set; }
        public object Category { get; internal set; }
        public object Key { get; internal set; }

        private object? _value;

        public object? Value
        {
            get => _value;
            set => SetProperty(ref _value, value);
        }
    }
}
