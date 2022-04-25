using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace UnityCommander.Controls.Ribbon.Subgroup
{
    public class ButtonsWrapGroup : BaseSubgroup
    {
        #region Override Members

        /// <summary>
        /// The arrange override.
        /// </summary>
        /// <param name="arrangeBounds">
        /// The arrange bounds.
        /// </param>
        /// <returns>
        /// The <see cref="Size"/>.
        /// </returns>
        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            foreach (UIElement child in this.Children)
            {
                child.Arrange(new Rect(new Point(0, 0), child.DesiredSize));
            }

            return arrangeBounds;
        }

        /// <summary>
        /// The measure override.
        /// </summary>
        /// <param name="availableSize">
        /// The available size.
        /// </param>
        /// <returns>
        /// The <see cref="Size"/>.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            Size size = new Size(0, 0);
            foreach (UIElement child in this.Children)
            {
                child.Measure(size);
            }

            return new Size(0, 0);
        }

        #endregion
    }
}
