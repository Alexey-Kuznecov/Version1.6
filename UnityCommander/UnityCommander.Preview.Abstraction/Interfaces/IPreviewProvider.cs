
using UnityCommander.Preview.Abstraction.Models;

namespace UnityCommander.Preview.Abstraction;

public interface IPreviewProvider
{
    string Id { get; }

    int Priority { get; }

    ValueTask<bool> CanPreviewAsync(
        PreviewRequest request,
        CancellationToken cancellationToken);

    Task<PreviewResult> CreatePreviewAsync(
        PreviewRequest request,
        CancellationToken cancellationToken);
}
