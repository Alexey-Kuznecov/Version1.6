using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommander.AI.ImageSearch
{
    public static class ImageSearchLogger
    {
        public static event Action<ImageSearchMessage>? OnLog;

        public static void Log(string message, ImageSearchLevel level = ImageSearchLevel.Info)
        {
            OnLog?.Invoke(new ImageSearchMessage(message, level, DateTime.Now));
        }
    }

    public record ImageSearchMessage(string Message, ImageSearchLevel Level, DateTime Timestamp);

    public enum ImageSearchLevel
    {
        Trace,
        Info,
        Warning,
        Error
    }
}
