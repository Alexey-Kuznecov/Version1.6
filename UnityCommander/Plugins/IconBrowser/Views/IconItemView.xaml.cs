
using IconBrowser.Models;
using System.Windows;
using System.Windows.Controls;

namespace IconBrowser.Views
{
    /// <summary>
    /// Логика взаимодействия для IconItemView.xaml
    /// </summary>
    public partial class IconItemView : UserControl
    {
        public static readonly DependencyProperty ViewModeProperty =
            DependencyProperty.Register(
            nameof(ViewMode),
            typeof(IconViewMode),
            typeof(IconItemView),
            new PropertyMetadata(IconViewMode.Tile, OnViewModeChanged));

        public IconViewMode ViewMode
        {
            get => (IconViewMode)GetValue(ViewModeProperty);
            set => SetValue(ViewModeProperty, value);
        }

        public IconItemView()
        {
            InitializeComponent();
        }

        private static void OnViewModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ListView list && e.NewValue is true)
            {
                list.SelectionMode = SelectionMode.Multiple; // отключаем стандартное выделение
                list.SelectedItem = null;
            }
        }
    }
}
