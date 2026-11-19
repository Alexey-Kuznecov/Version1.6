
using System.Windows.Controls;
using System.Windows.Input;
using UnityCommander.Preview.Wpf.ViewModels;

namespace UnityCommander.Preview.Wpf.Views
{
    /// <summary>
    /// Логика взаимодействия для VideoPreviewView.xaml
    /// </summary>
    public partial class VideoPreviewView : UserControl
    {
        public VideoPreviewView()
        {
            InitializeComponent();
        }

        private void Timeline_PreviewMouseDown(
            object sender,
            MouseButtonEventArgs e)
        {
            if (DataContext is VideoPreviewViewModel viewModel)
                viewModel.BeginSeek();
        }

        private void Timeline_PreviewMouseUp(
            object sender,
            MouseButtonEventArgs e)
        {
            if (DataContext is VideoPreviewViewModel viewModel)
                viewModel.EndSeek();
        }
    }
}
