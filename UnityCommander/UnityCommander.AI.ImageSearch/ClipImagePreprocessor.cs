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

        public static DenseTensor<float> Preprocess(string filePath)
        {
            using var image = Image.Load<Rgb24>(filePath);

            // Resize → 224x224
            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Size = new Size(ImageSize, ImageSize),
                Mode = ResizeMode.Crop
            }));

            var tensor = new DenseTensor<float>(new[] { 1, 3, ImageSize, ImageSize });

            for (int y = 0; y < ImageSize; y++)
            {
                for (int x = 0; x < ImageSize; x++)
                {
                    var pixel = image[x, y];

                    // Преобразуем в [0..1]
                    float r = pixel.R / 255f;
                    float g = pixel.G / 255f;
                    float b = pixel.B / 255f;

                    tensor[0, 0, y, x] = (r - Mean[0]) / Std[0];
                    tensor[0, 1, y, x] = (g - Mean[1]) / Std[1];
                    tensor[0, 2, y, x] = (b - Mean[2]) / Std[2];
                }
            }

            return tensor;
        }
    }
}
