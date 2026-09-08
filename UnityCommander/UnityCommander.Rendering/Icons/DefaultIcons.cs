
using UnityCommander.Abstractions.Icons;

namespace UnityCommander.Rendering.Icons
{
    public static class DefaultIcons
    {
        public static RuntimeIcon Missing { get; } = new()
        {
            Layers =
            [
                new RuntimeIconLayer
                {
                   Data = "M0,0 L16,0 L16,16 L0,16 Z",
                   Fill = "Brushes.LightGray",
                   Stroke = "Brushes.Gray",
                   StrokeWidth = 1,
                   StrokeLineJoin = "PenLineJoin.Miter"
                }
            ],
            Color = "LightGray",
            Stroke = "Gray",
            StrokeWidth = 1,
            IconType = IconType.Filled,
            Key = "MissingIcon"
        };
    }
}
