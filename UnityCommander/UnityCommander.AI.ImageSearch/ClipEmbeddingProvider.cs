using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;

namespace UnityCommander.AI.ImageSearch
{

    public class ClipEmbeddingProvider
    {
        private readonly InferenceSession _session;

        public ClipEmbeddingProvider(string modelPath)
        {
            _session = new InferenceSession(modelPath);
        }

        public float[] GetEmbedding(string imagePath)
        {
            var tensor = ClipImagePreprocessor.Preprocess(imagePath);

            var inputs = new List<NamedOnnxValue>
        {
            NamedOnnxValue.CreateFromTensor("input", tensor)
        };

            using var results = _session.Run(inputs);
            var embedding = results.First().AsEnumerable<float>().ToArray();

            return embedding;
        }
    }
}
