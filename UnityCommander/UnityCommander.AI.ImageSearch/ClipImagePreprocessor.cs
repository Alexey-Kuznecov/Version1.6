using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace UnityCommander.AI.ImageSearch
{
    using Microsoft.ML.OnnxRuntime.Tensors;

    public static class ClipImagePreprocessor
    {
        // Размер входа для CLIP
        private const int ImageSize = 224;

        // Нормализация как в CLIP
        private static readonly float[] Mean = { 0.48145466f, 0.4578275f, 0.40821073f };
        private static readonly float[] Std = { 0.26862954f, 0.26130258f, 0.27577711f };

        public static DenseTensor<float> PreprocessImageToTensor(string path)
        {
            using Image<Rgb24> image = Image.Load<Rgb24>(path);

            // 1. Resize to 224x224
            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Size = new Size(224, 224),
                Mode = ResizeMode.Crop
            }));

            // 2. Create tensor [1, 3, 224, 224]
            var tensor = new DenseTensor<float>(new[] { 1, 3, 224, 224 });

            float[] mean = { 0.485f, 0.456f, 0.406f };
            float[] std = { 0.229f, 0.224f, 0.225f };

            for (int y = 0; y < 224; y++)
            {
                for (int x = 0; x < 224; x++)
                {
                    Rgb24 pixel = image[x, y];

                    tensor[0, 0, y, x] = (pixel.R / 255f - mean[0]) / std[0];
                    tensor[0, 1, y, x] = (pixel.G / 255f - mean[1]) / std[1];
                    tensor[0, 2, y, x] = (pixel.B / 255f - mean[2]) / std[2];
                }
            }

            return tensor;
        }
    }
}
