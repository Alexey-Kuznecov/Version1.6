
using UnityCommander.Preview.Abstraction.Models;

namespace UnityCommander.Preview.Abstraction;

public interface IPreviewService
{
    Task<PreviewResult?> PreviewAsync(
        PreviewRequest request,
        CancellationToken cancellationToken = default);
}