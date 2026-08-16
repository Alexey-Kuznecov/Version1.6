
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace UnityCommander.Controls
{
    public class CircularProgress : ContentControl
    {
        public static readonly DependencyProperty ProgressProperty =
            DependencyProperty.Register(
                nameof(Progress),
                typeof(double),
                typeof(CircularProgress),
                new FrameworkPropertyMetadata(
                    0d,
                    FrameworkPropertyMetadataOptions.AffectsRender));

        public double Progress
        {
            get => (double)GetValue(ProgressProperty);
            set => SetValue(ProgressProperty, value);
        }

        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register(
                nameof(StrokeThickness),
                typeof(double),
                typeof(CircularProgress),
                new FrameworkPropertyMetadata(2d));

        public double StrokeThickness
        {
            get => (double)GetValue(StrokeThicknessProperty);
            set => SetValue(StrokeThicknessProperty, value);
        }


        public static readonly DependencyProperty ProgressBrushProperty =
            DependencyProperty.Register(
                nameof(ProgressBrush),
                typeof(Brush),
                typeof(CircularProgress),
                new FrameworkPropertyMetadata(null));

        public Brush? ProgressBrush
        {
            get => (Brush?)GetValue(ProgressBrushProperty);
            set => SetValue(ProgressBrushProperty, value);
        }


        public static readonly DependencyProperty TrackBrushProperty =
            DependencyProperty.Register(
                nameof(TrackBrush),
                typeof(Brush),
                typeof(CircularProgress),
                new FrameworkPropertyMetadata(null));

        public Brush? TrackBrush
        {
            get => (Brush?)GetValue(TrackBrushProperty);
            set => SetValue(TrackBrushProperty, value);
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            double size = Math.Min(ActualWidth, ActualHeight);

            if (size <= StrokeThickness)
                return;

            double radius = (size - StrokeThickness) / 2;

            var center = new Point(
                ActualWidth / 2,
                ActualHeight / 2);

            var trackPen = new Pen(
                TrackBrush ?? Brushes.Gray,
                StrokeThickness);

            dc.DrawEllipse(
                null,
                trackPen,
                center,
                radius,
                radius);

            if (Progress <= 0)
                return;

            var progressBrush =
                ProgressBrush ?? Brushes.DeepSkyBlue;

            var progressPen = new Pen(
                progressBrush,
                StrokeThickness);

            if (Progress >= 100)
            {
                dc.DrawEllipse(
                    null,
                    progressPen,
                    center,
                    radius,
                    radius);

                return;
            }

            double angle = Progress / 100.0 * 360.0;

            double startAngle = -90;
            double endAngle = startAngle + angle;

            Point start = GetPoint(center, radius, startAngle);
            Point end = GetPoint(center, radius, endAngle);

            bool largeArc = angle > 180;

            var geometry = new StreamGeometry();

            using (var ctx = geometry.Open())
            {
                ctx.BeginFigure(start, false, false);

                ctx.ArcTo(
                    end,
                    new Size(radius, radius),
                    0,
                    largeArc,
                    SweepDirection.Clockwise,
                    true,
                    false);
            }

            geometry.Freeze();

            dc.DrawGeometry(
                null,
                progressPen,
                geometry);
        }

        private static Point GetPoint(
            Point center,
            double radius,
            double angle)
        {
            double radians = angle * Math.PI / 180;

            return new Point(
                center.X + radius * Math.Cos(radians),
                center.Y + radius * Math.Sin(radians));
        }
    }
}
