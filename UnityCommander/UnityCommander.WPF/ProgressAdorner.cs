
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace UnityCommander.WPF
{
    public sealed class ProgressAdorner : Adorner
    {
        public double Progress
        {
            get => (double)GetValue(ProgressProperty);
            set => SetValue(ProgressProperty, value);
        }

        public ProgressIndicatorMode Mode { get; }

        public static readonly DependencyProperty ProgressProperty =
            DependencyProperty.Register(
                nameof(Progress),
                typeof(double),
                typeof(ProgressAdorner),
                new FrameworkPropertyMetadata(
                    0d,
                    FrameworkPropertyMetadataOptions.AffectsRender));

        public ProgressAdorner(
            UIElement adornedElement,
            ProgressIndicatorMode mode)
            : base(adornedElement)
        {
            Mode = mode;
            IsHitTestVisible = false;
        }

        protected override void OnRender(
            DrawingContext drawingContext)
        {
            switch (Mode)
            {
                case ProgressIndicatorMode.Linear:
                    DrawLinear(drawingContext);
                    break;

                case ProgressIndicatorMode.Border:
                    DrawBorder(drawingContext);
                    break;
            }
        }

        private void DrawLinear(DrawingContext dc)
        {
            var width = AdornedElement.RenderSize.Width;
            var height = AdornedElement.RenderSize.Height;

            var rect = new Rect(
                0,
                height - 2,
                width * Progress,
                2);

            dc.DrawRectangle(
                Brushes.DodgerBlue,
                null,
                rect);
        }

        private void DrawBorder(DrawingContext dc)
        {
            var rect = new Rect(
              1.5,
              1.5,
              AdornedElement.RenderSize.Width - 3,
              AdornedElement.RenderSize.Height - 3);

            var path = GetProgressGeometry(
                rect,
                Progress);

            var pen = new Pen(
                Brushes.DodgerBlue,
                2);

            dc.DrawGeometry(
                null,
                pen,
                path);
        }

        private static Geometry GetProgressGeometry(
            Rect rect,
            double progress)
        {
            progress = Math.Clamp(progress, 0, 1);

            var width = rect.Width;
            var height = rect.Height;
            var perimeter = 2 * (width + height);
            var length = perimeter * progress;

            var geometry = new StreamGeometry();

            using var context = geometry.Open();

            var remaining = length;

            // Верх
            var top = Math.Min(remaining, width);

            if (top > 0)
            {
                context.BeginFigure(
                    new Point(rect.Left, rect.Top),
                    false,
                    false);

                context.LineTo(
                    new Point(rect.Left + top, rect.Top),
                    true,
                    false);

                remaining -= top;
            }

            // Правая сторона
            var right = Math.Min(remaining, height);

            if (right > 0)
            {
                context.LineTo(
                    new Point(rect.Right, rect.Top + right),
                    true,
                    false);

                remaining -= right;
            }

            // Низ
            var bottom = Math.Min(remaining, width);

            if (bottom > 0)
            {
                context.LineTo(
                    new Point(rect.Right - bottom, rect.Bottom),
                    true,
                    false);

                remaining -= bottom;
            }

            // Левая сторона
            var left = Math.Min(remaining, height);

            if (left > 0)
            {
                context.LineTo(
                    new Point(rect.Left, rect.Bottom - left),
                    true,
                    false);
            }

            return geometry;
        }
    }
}
