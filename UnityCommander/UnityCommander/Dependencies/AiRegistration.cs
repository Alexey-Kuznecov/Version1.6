
using Prism.Ioc;
using System;
using System.IO;
using UnityCommander.AI.ImageSearch;

namespace UnityCommander.Dependencies
{
    public static class AiRegistration
    {
        public static void Register(IContainerRegistry registry)
        {
            string modelPath = Path.GetFullPath(
            Path.Combine(
                AppContext.BaseDirectory,
                @"..\..\..\..\Resources\ai_models\model.onnx"));

            registry.RegisterSingleton<IImageSimilarityService>(() =>
                new ImageSimilarityService(modelPath));
        }
    }
}
