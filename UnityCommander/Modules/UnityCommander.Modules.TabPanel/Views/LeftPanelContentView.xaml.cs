
using System.Windows.Controls;
using System.Windows.Media;

namespace UnityCommander.Modules.TabPanel.Views
{
    /// <summary>
    /// Interaction logic for ViewA.xaml
    /// </summary>
    public partial class LeftPanelContentView : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LeftPanelContentView"/> class.
        /// </summary>
        public LeftPanelContentView()
        {
            this.InitializeComponent();
        }

        //private void Border_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    var border = sender as Border;
        //    border.BorderBrush = new SolidColorBrush(Color.FromRgb(125, 162, 230));
        //    border.BorderThickness = new System.Windows.Thickness(0, 0, 1, 1);
        //}

        //private void Border_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    var border = sender as Border;
        //    border.BorderBrush = Brushes.White;
        //    //border.BorderThickness = new System.Windows.Thickness(0);
        //}
    }
}
