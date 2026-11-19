using LibVLCSharp.Shared;
using UnityCommander.Preview.Abstraction;
using UnityCommander.Preview.Abstraction.Models;
using UnityCommander.Preview.Abstraction.Models.Content;
using UnityCommander.Preview.Wpf.ViewModels;
using UnityCommander.Preview.Wpf.Views;

namespace UnityCommander.Preview.Wpf.Presenters;

public sealed class VideoPreviewPresenter : IPreviewPresenter
{
    private readonly VideoEngine _engine;

    public VideoPreviewPresenter(VideoEngine engine)
    {
        _engine = engine;
    }

    public string Id => "core.video";

    public int Priority => 100;

    public PreviewKind Kind => PreviewKind.Video;

    public object CreateView()
    {
        var viewModel = new VideoPreviewViewModel();

        var mediaPlayer = new MediaPlayer(_engine.LibVLC);

        viewModel.SetMediaPlayer(mediaPlayer);

        return new VideoPreviewView
        {
            DataContext = viewModel
        };
    }

    public void Present(object view, PreviewResult result)
    {
        if (view is not VideoPreviewView videoView ||
            videoView.DataContext is not VideoPreviewViewModel viewModel)
        {
            throw new ArgumentException("Invalid video preview view.");
        }

        if (result.Content is not VideoPreviewContent content)
        {
            throw new ArgumentException(
                "Preview result does not contain video content.");
        }

        viewModel.Stop();

        var media = new Media(_engine.LibVLC, content.Source);

        viewModel.MediaPlayer!.Media = media;
        viewModel.Play();
    }

    public void Clear(object view)
    {
        if (view is VideoPreviewView videoView &&
            videoView.DataContext is VideoPreviewViewModel viewModel)
        {
            viewModel.Stop();
            viewModel.MediaPlayer!.Media = null;
        }
    }
}