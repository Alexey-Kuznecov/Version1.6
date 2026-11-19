
using UnityCommander.Preview.Abstraction;
using UnityCommander.Preview.Abstraction.Models;

namespace UnityCommander.Preview;

public sealed class PreviewService : IPreviewService
{
    private readonly IPreviewRegistry _registry;

    public PreviewService(IPreviewRegistry registry)
    {
        _registry = registry ?? throw new ArgumentNullException(nameof(registry));
    }

    public async Task<PreviewResult?> PreviewAsync(
        PreviewRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var providers = _registry.Providers;

        foreach (var provider in providers)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var supported = await provider
                .CanPreviewAsync(request, cancellationToken)
                .ConfigureAwait(false);

            if (!supported)
                continue;

            cancellationToken.ThrowIfCancellationRequested();

            return await provider
                .CreatePreviewAsync(request, cancellationToken)
                .ConfigureAwait(false);
        }

        return null;
    }
}