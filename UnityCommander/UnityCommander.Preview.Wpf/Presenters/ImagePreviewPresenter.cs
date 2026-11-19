
using System.Windows.Media.Imaging;
using UnityCommander.Preview.Abstraction;
using UnityCommander.Preview.Abstraction.Models;
using UnityCommander.Preview.Abstraction.Models.Content;
using UnityCommander.Preview.Wpf.ViewModels;
using UnityCommander.Preview.Wpf.Views;

namespace UnityCommander.Preview.Wpf.Presenters;

public sealed class ImagePreviewPresenter : IPreviewPresenter
{
    public string Id => "core.image";

    public int Priority => 100;

    public PreviewKind Kind => PreviewKind.Image;

    public object CreateView()
    {
        return new ImagePreviewView
        {
            DataContext = new ImagePreviewViewModel()
        };
    }

    public void Present(object view, PreviewResult result)
    {
        if (view is not ImagePreviewView imageView ||
            imageView.DataContext is not ImagePreviewViewModel viewModel)
        {
            throw new ArgumentException(
                "Invalid image preview view.",
                nameof(view));
        }

        if (result.Content is not ImagePreviewContent content)
        {
            throw new ArgumentException(
                "Preview result does not contain image content.",
                nameof(result));
        }

        var bitmap = new BitmapImage();

        if (content.Data.CanSeek)
            content.Data.Position = 0;

        bitmap.BeginInit();
        bitmap.CacheOption = BitmapCacheOption.OnLoad;
        bitmap.StreamSource = content.Data;
        bitmap.EndInit();
        bitmap.Freeze();

        viewModel.SetImage(bitmap);
    }

    public void Clear(object view)
    {
        if (view is ImagePreviewView imageView &&
            imageView.DataContext is ImagePreviewViewModel viewModel)
        {
            viewModel.Clear();
        }
    }
}