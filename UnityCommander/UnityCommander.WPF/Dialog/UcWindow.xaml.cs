
using System.Windows;
using System.Windows.Input;

namespace UnityCommander.WPF.Dialog
{
    public partial class UcWindow : Window
    {
        public ICommand MinimizeCommand { get; }
        public ICommand MaximizeCommand { get; }
        public ICommand CloseCommand { get; }

        public UcWindow()
        {
            InitializeComponent();

            MinimizeCommand = new DelegateCommand(Minimize);
            MaximizeCommand = new DelegateCommand(ToggleMaximize);
            CloseCommand = new DelegateCommand(CloseWindow);
        }

        private void Minimize()
        {
            WindowState = WindowState.Minimized;
        }

        private void ToggleMaximize()
        {
            WindowState = WindowState == WindowState.Maximized
                ? WindowState.Normal
                : WindowState.Maximized;
        }

        private void CloseWindow()
        {
            Close();
        }
    }
}
