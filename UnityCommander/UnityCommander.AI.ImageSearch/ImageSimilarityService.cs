using Microsoft.ML.OnnxRuntime;
using System.Collections.Concurrent;

namespace UnityCommander.AI.ImageSearch
{
    public class ImageSimilarityService : IImageSimilarityService
    {
        private readonly ClipEmbeddingProvider _embeddings;

        // Кэш эмбеддингов — ускоряет поиск в 20–100 раз
        private readonly ConcurrentDictionary<string, float[]> _cache = new();

        public ImageSimilarityService(string modelPath)
        {
            _embeddings = new ClipEmbeddingProvider(modelPath);
        }

        public float[] GetEmbedding(string imagePath)
        {
            return _cache.GetOrAdd(imagePath, p => _embeddings.GetEmbedding(p));
        }

        public IEnumerable<(string path, float score)> FindSimilarImages(
            string targetImagePath,
            IEnumerable<string> candidateImagePaths,
            int top = 10)
        {
            var targetEmb = GetEmbedding(targetImagePath);

            var scores = candidateImagePaths
                .Where(f => File.Exists(f))
                .Select(f => (path: f, score: CosineSimilarity(targetEmb, GetEmbedding(f))))
                .OrderByDescending(x => x.score)
                .Take(top);

            return scores;
        }

        private static float CosineSimilarity(float[] v1, float[] v2)
        {
            float dot = 0, mag1 = 0, mag2 = 0;

            for (int i = 0; i < v1.Length; i++)
            {
                dot += v1[i] * v2[i];
                mag1 += v1[i] * v1[i];
                mag2 += v2[i] * v2[i];
            }

            return dot / (MathF.Sqrt(mag1) * MathF.Sqrt(mag2));
        }
    }
}
