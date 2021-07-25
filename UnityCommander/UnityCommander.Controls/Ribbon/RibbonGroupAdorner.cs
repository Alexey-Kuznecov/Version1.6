
namespace UnityCommander.Controls.Ribbon
{
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    /// <summary>
    /// The group adorner.
    /// </summary>
    public class RibbonGroupAdorner : Grid
    {
        /// <summary>
        /// The set adorner.
        /// </summary>
        /// <param name="ribbonGroup">
        /// The ribbon Group.
        /// </param>
        /// <returns>
        /// The <see cref="RibbonGroupAdorner"/>.
        /// </returns>
        public RibbonGroupAdorner SetAdorner(string groupName, RibbonGroup ribbonGroup)
        {
            TextBlock groupNameContainer = new TextBlock()
            {
                TextAlignment = TextAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Text = groupName
            };
           
            this.Children.Add(ribbonGroup);
            this.Children.Add(groupNameContainer);
            return this;
        }

        /// <summary>
        /// The arrange override.
        /// </summary>
        /// <param name="arrangeSize">
        /// The arrange size.
        /// </param>
        /// <returns>
        /// The <see cref="Size"/>.
        /// </returns>
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            double margin = 0;
            
            foreach (UIElement child in this.Children)
            {
                if (child is TextBlock groupName)
                {
                    groupName.Width = arrangeSize.Width - 2;
                }
               
                child.Arrange(new Rect(new Point(margin + 2, child is TextBlock ? 80 : 0), child.DesiredSize));
            }

            return arrangeSize;
        }

        private Size MeasureText(Label label, double width)
        {
            var formattedText = new FormattedText(
                label.Content.ToString(),
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(label.FontFamily, label.FontStyle, label.FontWeight, label.FontStretch),
                label.FontSize,
                Brushes.Black,
                new NumberSubstitution(),
                1);

            return new Size(formattedText.Width, formattedText.Height);
        }

        /// <summary>
        /// The measure override.
        /// </summary>
        /// <param name="constraint">
        /// The constraint.
        /// </param>
        /// <returns>
        /// The <see cref="Size"/>.
        /// </returns>
        protected override Size MeasureOverride(Size constraint)
        {
            double width = 0;
            double height = 0;

            Size size = new Size(double.PositiveInfinity, double.PositiveInfinity);
            foreach (UIElement child in this.Children)
            {
                child.Measure(size);

                if (child is RibbonGroup)
                {
                    height = child.DesiredSize.Height;
                    width += child.DesiredSize.Width;
                }
            }

            return new Size(width, height);
        }
    }
}
