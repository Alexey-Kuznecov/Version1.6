
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UnityCommander.Abstractions.Keyboard;
using UnityCommander.Modules.SettingsPanel.ViewModels;
using UnityCommander.WPF.Behaviors;

namespace UnityCommander.Modules.SettingsPanel.Editors
{
    /// <summary>
    /// Логика взаимодействия для ShortcutEditor.xaml
    /// </summary>
    public partial class ShortcutEditor : UserControl
    {
        public ShortcutEditor()
        {
            InitializeComponent();


            //this.Loaded += OnLoaded;
            //this.PreviewKeyDown += OnKeyDown;
        }

        //private void OnKeyDown(object sender, KeyEventArgs e)
        //{
        //    var vm = (ShortcutEditorViewModel)DataContext;

        //    vm.IsRecording = true;

        //    var modifiers = Keyboard.Modifiers;
   
        //    var (key, mod) = WpfShortcutConverter.FromKeyGesture(e.Key, modifiers);

        //    vm.Value = new ShortcutOverride
        //    {
        //        CommandId = vm.Value.CommandId,
        //        Key = key,
        //        Modifiers = mod
        //    };

        //    vm.IsRecording = false;

        //    e.Handled = true;
        //}

        //private void OnLoaded(object sender, RoutedEventArgs e)
        //{
        //    Focus();
        //}
    }
}
