
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace UnityCommander.Modules.FilePanel.Controllers.DnD.Adorders
{
    public class DragAdorner : Adorner
    {
        private readonly VisualBrush _brush;
        private readonly double _opacity;
        private readonly ContentPresenter _content;

        public DragAdorner(UIElement element, object data, DataTemplate template)
        : this(element)
        {
            _content = new ContentPresenter
            {
                Content = data,
                ContentTemplate = template,
                Opacity = 0.6,
                IsHitTestVisible = false
            };

            AddVisualChild(_content);
        }

        private DragAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            _brush = new VisualBrush(adornedElement)
            {
                Opacity = 0.6,
                Stretch = Stretch.None
            };

            IsHitTestVisible = false;
        }

        protected override void OnRender(DrawingContext dc)
        {
            var rect = new Rect(AdornedElement.RenderSize);

            dc.DrawRectangle(_brush, null, rect);
        }
    }
}
