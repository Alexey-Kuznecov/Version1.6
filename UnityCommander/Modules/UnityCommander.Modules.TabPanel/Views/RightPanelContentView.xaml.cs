using System.Windows.Controls;
using System.Windows.Media;

namespace UnityCommander.Modules.TabPanel.Views
{
    /// <summary>
    /// Interaction logic for ViewB.xaml
    /// </summary>
    public partial class RightPanelContentView : UserControl
    {
        public RightPanelContentView()
        {
            this.InitializeComponent();
        }

        //private void Border_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    var border = sender as Border;
        //    border.BorderBrush = new SolidColorBrush(Color.FromRgb(125, 162, 230));
        //    border.BorderThickness = new System.Windows.Thickness(1, 0, 0, 1);
        //}

        //private void Border_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    var border = sender as Border;
        //    border.BorderBrush = Brushes.White;
        //    //border.BorderThickness = new System.Windows.Thickness(0);
        //}
    }
}
