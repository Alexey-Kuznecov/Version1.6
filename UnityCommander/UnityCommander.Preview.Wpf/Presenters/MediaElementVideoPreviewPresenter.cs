
using UnityCommander.Preview.Abstraction;
using UnityCommander.Preview.Abstraction.Models;
using UnityCommander.Preview.Abstraction.Models.Content;
using UnityCommander.Preview.Wpf.Views;

namespace UnityCommander.Preview.Wpf.Presenters;

public sealed class MediaElementVideoPreviewPresenter : IPreviewPresenter
{
    public string Id => "core.video.mediaelement";

    public int Priority => 101;

    public PreviewKind Kind => PreviewKind.Video;

    public object CreateView()
    {
        var viewModel = new MediaElementVideoPreviewViewModel();

        var view = new MediaElementVideoPreviewView
        {
            DataContext = viewModel
        };

        viewModel.Attach(view.VideoPlayer);

        return view;
    }

    public void Present(object view, PreviewResult result)
    {
        if (view is not MediaElementVideoPreviewView videoView ||
            videoView.DataContext is not MediaElementVideoPreviewViewModel viewModel)
        {
            throw new ArgumentException("Invalid MediaElement video preview view.");
        }

        if (result.Content is not VideoPreviewContent content)
            throw new ArgumentException("Preview result does not contain video content.");

        viewModel.SetSource(content.Source);
    }

    public void Clear(object view)
    {
        if (view is MediaElementVideoPreviewView videoView &&
            videoView.DataContext is MediaElementVideoPreviewViewModel viewModel)
        {
            viewModel.Clear();
        }
    }
}

