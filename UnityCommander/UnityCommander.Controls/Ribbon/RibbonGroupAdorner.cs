
namespace UnityCommander.Controls.Ribbon
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    public class RibbonGroupAdorner : Panel
    {
        public RibbonGroupAdorner SetAdorner(string groupName, RibbonGroup ribbonGroup)
        {
            TextBlock groupNameContainer = new TextBlock()
            {
                TextAlignment = TextAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Text = groupName,
                Foreground = new SolidColorBrush(Color.FromArgb(130, 130, 130, 130)),
                Margin = new Thickness(3, 18, 0, 0)
            };
            
            this.InternalChildren.Add(ribbonGroup);
            this.InternalChildren.Add(groupNameContainer);

            return this;
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            double margin = 0;

            foreach (UIElement child in this.InternalChildren)
            {
                child.Arrange(new Rect(new Point(margin + 2, child is TextBlock ? 80 : 0), child.DesiredSize));
            }

            return arrangeSize;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            double width = 0;
            double height = 0;

            Size size = new Size(double.PositiveInfinity, double.PositiveInfinity);
            foreach (UIElement child in this.InternalChildren)
            {
                if (child is TextBlock groupName)
                    groupName.Width = width - 2;

                child.Measure(size);

                if (child is not RibbonGroup) continue;
                height = child.DesiredSize.Height;
                width += child.DesiredSize.Width;
            }

            return new Size(width, height);
        }
    }
}
