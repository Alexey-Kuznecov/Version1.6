
using System.Windows;
using System.Windows.Controls;

namespace UnityCommander.Controls.Layout
{
    public class LayoutPresenter : Panel
    {
        public static readonly DependencyProperty RootProperty =
            DependencyProperty.Register(
                nameof(Root),
                typeof(LayoutNode),
                typeof(LayoutPresenter),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        public LayoutNode Root
        {
            get => (LayoutNode)GetValue(RootProperty);
            set => SetValue(RootProperty, value);
        }

        private LayoutResult _layout;

        private readonly LayoutEngine _engine = new LayoutEngine();

        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (UIElement child in Children)
                child.Measure(availableSize);

            return availableSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (Root == null)
                return finalSize;

            _engine.Measure(Root, new Rect(0, 0, finalSize.Width, finalSize.Height));

            ApplyLayout(Root);

            return finalSize;
        }

        private void ApplyLayout(LayoutNode node)
        {
            //if (node is ILayoutVisual v &&
            //    _engine.LayoutMap.TryGetValue(node, out var rect))
            //{
            //    v.Arrange(rect);
            //}

            switch (node)
            {
                case SplitNode s:
                    ApplyLayout(s.First);
                    ApplyLayout(s.Second);
                    break;

                case StackNode st:
                    foreach (var c in st.Children)
                        ApplyLayout(c);
                    break;

                case FixedNode f:
                    if (f.Content is LayoutNode ln)
                        ApplyLayout(ln);
                    break;
            }
        }
    }
}
