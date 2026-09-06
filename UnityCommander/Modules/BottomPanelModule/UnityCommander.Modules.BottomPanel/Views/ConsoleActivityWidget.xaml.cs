
using System.Reflection.PortableExecutable;
using System.Windows;
using System.Windows.Controls;

namespace UnityCommander.Modules.BottomPanel.Views
{
    /// <summary>
    /// Логика взаимодействия для ConsoleActivityWidget.xaml
    /// </summary>
    public partial class ConsoleActivityWidget : UserControl
    {
        public ConsoleActivityWidget()
        {
            InitializeComponent();
        }

        private bool _isExpanded = true;

        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                if (_isExpanded == value)
                    return;

                _isExpanded = value;
                UpdateExpandedState();
            }
        }

        private void ToggleExpanded(object sender, RoutedEventArgs e)
        {
            IsExpanded = !IsExpanded;
        }

        private void UpdateExpandedState()
        {
            Details.Visibility =
                IsExpanded
                    ? Visibility.Visible
                    : Visibility.Collapsed;

            WidgetGrid.Width =
                IsExpanded
                    ? 300
                    : 100;

            CollapseButton.Content =
                IsExpanded ? "˄" : "˅";
        }
    }
}
