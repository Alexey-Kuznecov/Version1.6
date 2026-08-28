
namespace UnityCommander.AI.ImageSearch
{
    public interface IImageSimilarityService
    {
        float[] GetEmbedding(string imagePath);
        public IEnumerable<(string path, float score)> FindSimilarImages(
            string targetImagePath,
            IEnumerable<string> candidateImagePaths,
            int top = 10);
    }
}
