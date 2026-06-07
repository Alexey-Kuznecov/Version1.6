
namespace UnityCommander.Modules.FilePanel.Behaviors
{
    using System.Windows;
    using System.Windows.Documents;
    using System.Windows.Media;

    public class MultiSelectionMouseAdorner : Adorner
    {
        public static readonly DependencyProperty HighlightAreaProperty = DependencyProperty.Register(
            "HighlightArea", typeof(Rect), typeof(MultiSelectionMouseAdorner), new FrameworkPropertyMetadata(new Rect(), FrameworkPropertyMetadataOptions.AffectsRender));
        private Size adornedDesiredSize;
        public MultiSelectionMouseAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            if (adornedElement is AdornerLayer adorned)
            {
                this.adornedDesiredSize = new Size(adorned.ActualWidth, adorned.ActualHeight);
            }

            this.BackgroundBrush = new SolidColorBrush(Colors.LightBlue);
            this.BackgroundBrush.Opacity = 0.5;
            this.PenBrush = new SolidColorBrush(Colors.Black);
            this.PenBrush.Opacity = 0.5;
            this.DrawingPen = new Pen(this.PenBrush, 1);
            this.IsHitTestVisible = false;
        }
        public SolidColorBrush BackgroundBrush { get; set; }
        public SolidColorBrush PenBrush { get; set; }
        public Pen DrawingPen { get; set; }
        public Rect HighlightArea
        {
            get => (Rect)this.GetValue(HighlightAreaProperty);
            set => this.SetValue(HighlightAreaProperty, value);
        }
        protected override void OnRender(DrawingContext dc)
        {
            dc.DrawRectangle(this.BackgroundBrush, this.DrawingPen, this.HighlightArea);
        }
    }
}
