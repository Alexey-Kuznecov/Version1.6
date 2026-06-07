
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Media;

namespace UnityCommander.Modules.BottomPanel.Highlighting
{
    public sealed class GuidHighlightRule : ILogHighlightRule
    {
        private static readonly Brush[] Palette =
        [
            Brushes.Gold,
            Brushes.LimeGreen,
            Brushes.DeepSkyBlue,
            Brushes.Orange,
            Brushes.Violet,
            Brushes.Cyan,
            Brushes.HotPink,
            Brushes.Turquoise,
            Brushes.YellowGreen,
            Brushes.Coral,
            Brushes.Salmon,
            Brushes.MediumPurple,
            Brushes.Aqua,
            Brushes.Khaki,
            Brushes.SpringGreen,
            Brushes.Plum,
            Brushes.LightSeaGreen,
            Brushes.LightSkyBlue,
            Brushes.PaleGreen,
            Brushes.DarkOrange
        ];

        private readonly Dictionary<Guid, Brush> _colors = new();

        private int _nextColor;

        public Regex Pattern { get; } =
            new(
                @"\b[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}\b",
                RegexOptions.Compiled);

        public HighlightStyle GetStyle(string guid)
        {
            return new HighlightStyle(
                    GetGuidColor(guid), 
                    Brushes.Black);
        }

        public Brush GetGuidColor(string guid)
        {
            if (!Guid.TryParse(guid, out var parsedGuid))
                return Brushes.White;

            if (_colors.TryGetValue(parsedGuid, out var brush))
                return brush;

            brush = Palette[_nextColor % Palette.Length];

            _colors[parsedGuid] = brush;

            _nextColor++;

            return brush;
        }
    }
}
