
using System.Windows.Controls;
using System.Windows.Input;

namespace UnityCommander.Preview.Wpf.Views
{
    /// <summary>
    /// Логика взаимодействия для MediaElementVideoPreviewView.xaml
    /// </summary>
    public partial class MediaElementVideoPreviewView : UserControl
    {
        public MediaElementVideoPreviewView()
        {
            InitializeComponent();
        }

        private void Timeline_PreviewMouseDown(
           object sender,
           MouseButtonEventArgs e)
        {
            if (DataContext is MediaElementVideoPreviewViewModel viewModel)
                viewModel.BeginSeek();
        }

        private void Timeline_PreviewMouseUp(
            object sender,
            MouseButtonEventArgs e)
        {
            if (DataContext is MediaElementVideoPreviewViewModel viewModel)
                viewModel.EndSeek();
        }

        private void VideoSurface_MouseMove(object sender, MouseEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("VIDEO MOUSE MOVE");
        }

        private void VideoSurface_MouseLeave(object sender, MouseEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("VIDEO MOUSE LEAVE");
        }
    }
}
