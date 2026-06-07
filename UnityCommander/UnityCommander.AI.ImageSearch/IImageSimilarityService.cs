using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
