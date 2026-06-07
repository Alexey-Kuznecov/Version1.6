using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using System.Diagnostics;

namespace UnityCommander.AI.ImageSearch
{

    public class ClipEmbeddingProvider
    {
        private readonly InferenceSession _session;

        public ClipEmbeddingProvider(string modelPath)
        {
            try
            {
               _session = new InferenceSession(modelPath);
                Debug.WriteLine("MODEL INPUTS:", ImageSearchLevel.Trace);
                foreach (var inp in _session.InputMetadata)
                    Debug.WriteLine($"  {inp.Key} : {string.Join(",", inp.Value.Dimensions)}");

                Debug.WriteLine("MODEL OUTPUTS:", ImageSearchLevel.Trace);
                foreach (var outp in _session.OutputMetadata)
                    Debug.WriteLine($"  {outp.Key} : {string.Join(",", outp.Value.Dimensions)}");
            }
            catch (OnnxRuntimeException ex)
            {
                Debug.WriteLine("ONNX Error: " + ex.Message, ImageSearchLevel.Error);
            }
        }

        public float[] GetEmbedding(string imagePath)
        {
            ImageSearchLogger.Log("DEBUG: Preprocess...");
            var tensor = ClipImagePreprocessor.PreprocessImageToTensor(imagePath);

            var inputs = new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor("pixel_values", tensor)
            };

            ImageSearchLogger.Log("DEBUG: Inference...", ImageSearchLevel.Trace);

            using var results = _session.Run(inputs);
            ImageSearchLogger.Log("DEBUG: Extract output...", ImageSearchLevel.Trace);
            var embedding = results.First().AsEnumerable<float>().ToArray();

            return embedding;
        }
    }

    //public class ClipEmbeddingProvider2
    //{
    //    private readonly InferenceSession _session;
    //    private readonly string _inputName;
    //    private readonly string _outputName;

    //    public ClipEmbeddingProvider2(string modelPath)
    //    {
    //        _session = new InferenceSession(modelPath);

    //        _inputName = _session.InputMetadata.Keys.First();
    //        _outputName = _session.OutputMetadata.Keys.First();

    //        Console.WriteLine($"CLIP INPUT:  {_inputName}");
    //        Console.WriteLine($"CLIP OUTPUT: {_outputName}");
    //    }

    //    public float[] GetEmbedding(string imagePath)
    //    {
    //        Console.WriteLine("DEBUG: Preprocess...");
    //        var tensor = ClipImagePreprocessor.Preprocess(imagePath);

    //        var inputs = new[]
    //        {
    //        NamedOnnxValue.CreateFromTensor(_inputName, tensor)
    //    };

    //        Console.WriteLine("DEBUG: Inference...");
    //        using var results = _session.Run(inputs);

    //        Console.WriteLine("DEBUG: Extract output...");
    //        var embedding = results[_outputName].AsEnumerable<float>().ToArray();

    //        return embedding;
    //    }
    //}
}
