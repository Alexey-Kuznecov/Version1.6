

using UnityCommander.Preview.Abstraction.Models;

namespace UnityCommander.Preview.Abstraction;

public interface IPreviewRegistry
{
    IReadOnlyList<IPreviewProvider> Providers { get; }

    IReadOnlyList<IPreviewPresenter> Presenters { get; }

    void RegisterProvider(string ownerId, IPreviewProvider provider);

    void RegisterPresenter(string ownerId, IPreviewPresenter presenter);

    IPreviewPresenter? FindPresenter(PreviewResult result);

    void Cleanup(string ownerId);
}