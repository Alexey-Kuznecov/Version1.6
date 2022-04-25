using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace AIconBrowser.Components.InputBox
{
    /// <summary>
    /// Логика взаимодействия для InputBox.xaml
    /// </summary>
    public partial class InputBox : Window
    {
        public InputBox()
        {
            InitializeComponent();
        }
        private static InputBox _inputBox;
        public static void Show(ICommand action, Actions actionType, string placeholder = "")
        {
            // Intializaion inputbox by constructor argument
            _inputBox = new InputBox();
            var inputBoxViewModel = new InputBoxViewModel(action, actionType, placeholder);
            _inputBox.DataContext = inputBoxViewModel;
            _inputBox.ShowDialog();
        }
        public new static void Close()
        {
            ((Window) _inputBox).Close();
        }
        public static void AddForditWord(List<string> word)
        {
            InputBoxViewModel.ForbidWord = word;
        }
    }
}
