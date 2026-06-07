using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
