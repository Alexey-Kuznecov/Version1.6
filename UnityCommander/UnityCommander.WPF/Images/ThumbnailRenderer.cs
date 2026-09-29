
using UnityCommander.Abstractions.Thumbnail;

namespace UnityCommander.WPF.Images
{
    public sealed class ThumbnailRenderer : IThumbnailRenderer
    {
        private readonly IReadOnlyList<IThumbnailProvider> _providers;

        public ThumbnailRenderer(
            IEnumerable<IThumbnailProvider> providers)
        {
            _providers = providers
                .OrderByDescending(x => x.Priority)
                .ToList();
        }

        public async Task<ThumbnailResult?> RenderAsync(
            string path,
            ThumbnailRequest request,
            CancellationToken token)
        {
            foreach (var provider in _providers)
            {
                if (provider is FolderThumbnailProvider)
                    continue;

                if (!provider.CanProvide(path))
                    continue;

                var result = await provider.GetAsync(
                    path,
                    request,
                    token);

                if (result is not null)
                    return result;
            }

            return null;
        }
    }
}
