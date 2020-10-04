
namespace UnityCommander.Modules.FilePanel.Controls
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;

    using UnityCommander.Integration.Contracts;

    /// <summary>
    /// Knows how to perform the operations associated with carrying out the request.
    /// </summary>
    public class NavigationPanel : Panel, INavigator
    {
        /// <summary>
        /// The singleton.
        /// </summary>
        private static byte singleton;

        /// <summary>
        /// The margin.
        /// </summary>
        private static double margin;

        /// <summary>
        /// The bind.
        /// </summary>
        private static Binding bind;

        /// <summary>
        /// The removed.
        /// </summary>
        private readonly List<UIElement> removed;

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationPanel"/> class.
        /// </summary>
        public NavigationPanel()
            : base()
        {
            bind = new Binding("InternalChildren") { Source = this };
            singleton++;

            this.removed = new List<UIElement>();
        }

        /// <summary>
        /// The back.
        /// </summary>
        /// <param name="path">
        /// The <c>path</c>.
        /// </param>
        public void Back(string path)
        {
            if (bind.Source is FilePanel.Navigator navigator)
            {
            }
        }

        /// <summary>
        /// The next.
        /// </summary>
        /// <param name="path"> The <c>path</c>. </param>
        public void Next(string path)
        {
            if (bind.Source is Navigator navigator)
            {
                InternalChildren.Clear();
                margin = 0;

                string[] slp = path.Split('\\');
                foreach (var item in slp)
                {
                    Button button = new Button
                    {
                        Height = 25,
                        Content = item
                    };
                    InternalChildren.Add(button);
                }
            }
        }

        /// <summary>
        /// When overridden in a derived class, measures the size in layout required
        /// for child elements and determines a size for the FrameworkElement-derived class.
        /// </summary>
        /// <param name="availableSize">
        /// The available size that this element can give to child elements.
        /// Infinity can be specified as a value to indicate that the element will
        /// size to whatever content is available.
        /// </param>
        /// <returns>
        /// The size that this element determines it needs during layout, based on its calculations of child element sizes.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            Size size = new Size(double.PositiveInfinity, double.PositiveInfinity);

            // In our example, we just have one child. 
            // Report that our panel requires just the size of its only child.
            foreach (UIElement child in this.InternalChildren)
            {
                child.Measure(size);
            }

            return new Size();
        }

        /// <summary>
        /// When overridden in a derived class, positions child elements and determines
        /// a size for a FrameworkElement derived class.
        /// </summary>
        /// <param name="finalSize">
        /// The final area within the parent that this
        /// element should use to arrange itself and its children.
        /// </param>
        /// <returns>
        /// The actual size used.
        /// </returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            margin = 0;

            for (var index = 0; index < this.InternalChildren.Count; index++)
            {
                UIElement child = this.InternalChildren[index];
                child.Arrange(new Rect(new Point(margin, 10), child.DesiredSize));
                margin += child.DesiredSize.Width + 2;

                if (margin - 10 > finalSize.Width)
                {
                    this.removed.Add(child);
                    this.InternalChildren.RemoveAt(0);
                }
            }

            // Returns the final Arranged size
            return finalSize;
        }
    }
}
