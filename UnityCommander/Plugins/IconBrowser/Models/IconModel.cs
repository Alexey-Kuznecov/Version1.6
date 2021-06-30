using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using AIconBrowser.Help;

namespace AIconBrowser.Models
{
    public class IconModel
    {
        #region Constructor

        public IconModel() {}
        public IconModel(string name, string fg, string bg, string path)
        {
            Name = name;
            FgroundColor = HelperFunctions.StringFormatToSolidColor(fg);
            BgroundColor = HelperFunctions.StringFormatToSolidColor(bg);
            Path = new Path { Data = Geometry.Parse(path) };
            //StringPath = path;
            DrawIcon();
        }
        public IconModel(string name, SolidColorBrush fg, SolidColorBrush bg)
        {
            Name = name;
            FgroundColor = fg;
            BgroundColor = bg;
            DrawIcon();
        }
        public IconModel(string name, Path path, SolidColorBrush fg, SolidColorBrush bg)
        {
            Name = name;
            Path = path;
            FgroundColor = fg;
            BgroundColor = bg;
        }

        #endregion

        #region Properties

        public ushort Id { get; set; }
        public string StringPath { get; set; }
        public string Name { get; set; }
        public string CollectionName { get; set; }
        public int Scale { get; set; }
        public DrawingBrush Brush { get; set; }
        public SolidColorBrush BgroundColor { get; set; }
        public SolidColorBrush FgroundColor { get; set; }
        public Path Path { get; set; }
        public List<Path> PathList { get; set; }

        #endregion

        #region Functions

        /// <summary>
        /// Устанавливает иконку, использует ресурсы иконок.
        /// Устанавливает цвет кисти.
        /// </summary>
        private void DrawIcon()
        {
            Brush = (DrawingBrush)Application.Current.TryFindResource(Name);
            DrawingBrush dBrush = Brush?.Clone();
            DrawingGroup group = dBrush?.Drawing as DrawingGroup;
            if (group != null)
            {
                foreach (var child in group.Children)
                {
                    GeometryDrawing geometry = child as GeometryDrawing;
                    if (geometry != null) geometry.Brush = FgroundColor;
                }
            }
            Brush = dBrush;
        }
        /// <summary>
        /// Конвертирует кисть в полигоны.
        /// </summary>
        /// <param name="brush"></param>
        private void ConvertBrushToPath(DrawingBrush brush)
        {
            PathList = new List<Path>();
            var drawingGroup = (DrawingGroup) brush?.Drawing.CloneCurrentValue();

            if (drawingGroup != null)
            {
                foreach (var child in drawingGroup.Children)
                {
                    var geo = child as GeometryDrawing;
                    PathList.Add(new Path { Data = geo?.Geometry });
                }
            }
        }
        #endregion
    }
}
