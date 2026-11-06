
namespace UnityCommander.Abstractions.Preview
{
    public interface IPreviewProvider
    {
        string Id { get; }

        int Priority { get; }

        //bool CanPreview(PreviewRequest request);

        //Task<IPreviewContent> CreateAsync(
        //    PreviewRequest request,
        //    CancellationToken cancellationToken);
    }
}
