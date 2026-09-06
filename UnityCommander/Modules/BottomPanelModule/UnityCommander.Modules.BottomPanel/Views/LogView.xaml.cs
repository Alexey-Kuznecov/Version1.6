
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace UnityCommander.Modules.BottomPanel.Views
{
    /// <summary>
    /// Логика взаимодействия для LogTabView.xaml
    /// </summary>
    public partial class LogView : UserControl
    {
        public LogView()
        {
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (sender is ListBox lb)
            {
                ((INotifyCollectionChanged)lb.Items).CollectionChanged += (_, __) =>
                {
                    if (lb.Items.Count > 0)
                        lb.ScrollIntoView(lb.Items[^1]);
                };
            }
        }
    }
}
