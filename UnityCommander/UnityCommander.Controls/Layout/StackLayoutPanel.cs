
using System;
using System.Windows;
using System.Windows.Controls;

namespace UnityCommander.Controls.Layout
{
    public class StackLayoutPanel : Panel
    {
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register(
                nameof(Orientation),
                typeof(Orientation),
                typeof(StackLayoutPanel),
                new FrameworkPropertyMetadata(
                    Orientation.Vertical,
                    FrameworkPropertyMetadataOptions.AffectsMeasure |
                    FrameworkPropertyMetadataOptions.AffectsArrange));

        public Orientation Orientation
        {
            get => (Orientation)GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            double totalWidth = 0;
            double totalHeight = 0;

            foreach (UIElement child in Children)
            {
                if (child == null)
                    continue;

                Size constraint;

                if (Orientation == Orientation.Vertical)
                {
                    constraint = new Size(
                        availableSize.Width,
                        Math.Max(0, availableSize.Height - totalHeight));
                }
                else
                {
                    constraint = new Size(
                        Math.Max(0, availableSize.Width - totalWidth),
                        availableSize.Height);
                }

                child.Measure(constraint);

                if (Orientation == Orientation.Vertical)
                {
                    totalHeight += child.DesiredSize.Height;
                    totalWidth = Math.Max(totalWidth, child.DesiredSize.Width);
                }
                else
                {
                    totalWidth += child.DesiredSize.Width;
                    totalHeight = Math.Max(totalHeight, child.DesiredSize.Height);
                }
            }

            return new Size(totalWidth, totalHeight);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double offset = 0;

            for (int i = 0; i < Children.Count; i++)
            {
                var child = Children[i];

                bool last = i == Children.Count - 1;

                double height;

                if (last)
                    height = finalSize.Height - offset;
                else
                    height = child.DesiredSize.Height;

                child.Arrange(new Rect(
                    0,
                    offset,
                    finalSize.Width,
                    height));

                offset += height;
            }

            return finalSize;
        }
    }
}
