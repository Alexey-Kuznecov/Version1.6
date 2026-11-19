
using UnityCommander.Preview.Abstraction.Models;

namespace UnityCommander.Preview.Abstraction;

public interface IPreviewPresenter
{
    string Id { get; }

    int Priority { get; }

    PreviewKind Kind { get; }

    object CreateView();

    void Present(object view, PreviewResult result);

    void Clear(object view);
}