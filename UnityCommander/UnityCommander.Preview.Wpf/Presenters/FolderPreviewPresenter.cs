
using UnityCommander.Preview.Abstraction;
using UnityCommander.Preview.Abstraction.Models;
using UnityCommander.Preview.Wpf.ViewModels;
using UnityCommander.Preview.Wpf.Views;

namespace UnityCommander.Preview.Wpf.Presenters;
public sealed class FolderPreviewPresenter : IPreviewPresenter
{
    public string Id => "core.folder";

    public int Priority => 100;

    public PreviewKind Kind => PreviewKind.Folder;

    public object CreateView()
    {
        return new FolderPreviewView
        {
            DataContext = new FolderPreviewViewModel()
        };
    }

    public void Present(object view, PreviewResult result)
    {
        if (view is not FolderPreviewView folderView ||
            folderView.DataContext is not FolderPreviewViewModel viewModel)
        {
            throw new ArgumentException(
                "Invalid folder preview view.",
                nameof(view));
        }

        if (result.Content is not FolderPreviewContent content)
        {
            throw new ArgumentException(
                "Preview result does not contain folder content.",
                nameof(result));
        }

        viewModel.SetFolder(content);
    }

    public void Clear(object view)
    {
        if (view is FolderPreviewView folderView &&
            folderView.DataContext is FolderPreviewViewModel viewModel)
        {
            viewModel.Clear();
        }
    }
}
