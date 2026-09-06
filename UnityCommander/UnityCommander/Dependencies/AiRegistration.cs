
using Prism.Ioc;
using System;
using System.IO;
using UnityCommander.AI.ImageSearch;
using UnityCommander.Common;

namespace UnityCommander.Dependencies
{
    public static class AiRegistration
    {
        public static void Register(IContainerRegistry registry)
        {
            registry.RegisterSingleton<IImageSimilarityService>(sp =>
            {
                var paths = sp.Resolve<UnityCommanderPath>();

                var modelPath = Path.GetFullPath(
                    Path.Combine(
                        paths.ResourcesDirectory,
                        "ai_models",
                        "model.onnx"));

                return new ImageSimilarityService(modelPath);
            });
        }
    }
}
