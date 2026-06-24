
using System.Windows.Controls;
using System.Windows.Input;
using UnityCommander.Abstractions.Keyboard;
using UnityCommander.Modules.SettingsPanel.ViewModels;
using UnityCommander.WPF.Behaviors;

namespace UnityCommander.Modules.SettingsPanel.Views
{
    /// <summary>
    /// Interaction logic for SettingsPanelView.xaml
    /// </summary>
    public partial class SettingsPanelView : UserControl
    {
        public SettingsPanelView()
        {
            InitializeComponent();
            //this.Loaded += (_, __) => Focus();
            //this.PreviewKeyDown += OnKeyDown;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            //var vm = (ShortcutEditorViewModel)DataContext;

            //vm.IsRecording = true;

            //var modifiers = Keyboard.Modifiers;

            //var (key, mod) = WpfShortcutConverter.FromKeyGesture(e.Key, modifiers);

            //vm.Value = new ShortcutOverride
            //{
            //    CommandId = vm.Value.CommandId,
            //    Key = key,
            //    Modifiers = mod
            //};

            //vm.IsRecording = false;

            //e.Handled = true;
        }
    }
}
