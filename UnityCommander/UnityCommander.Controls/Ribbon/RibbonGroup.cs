
namespace UnityCommander.Controls.Ribbon
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    public class RibbonGroup : Panel
    {
        private static double margin;
        private Size containerGroupWidth;

        protected override void OnRender(DrawingContext dc)
        {
            SolidColorBrush mySolidColorBrush = new SolidColorBrush(Color.FromRgb(243, 243, 243));
            Pen myPen = new Pen(new SolidColorBrush(Color.FromArgb(255, 229, 228, 229)), 2);
            Rect myRect = new Rect(0, 1, this.containerGroupWidth.Width, 115);
            dc.DrawRectangle(mySolidColorBrush, myPen, myRect);
        }
        
        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            margin = 0;

            foreach (UIElement child in this.Children)
            {
                child.Arrange(new Rect(new Point(margin, 0), child.DesiredSize));
                margin += child.DesiredSize.Width;
            }
            
            return arrangeBounds;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            double width = 0;
            double height = 0;

            Size size = new Size(170, 125);
            foreach (UIElement child in this.Children)
            {
                child.Measure(size);
                height = child.DesiredSize.Height;
                width += child.DesiredSize.Width;
            }

            this.containerGroupWidth = new Size(width + 2, height);
            return this.containerGroupWidth;
        }
    }
}
