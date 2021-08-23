
namespace UnityCommander.Modules.FilePanel.Behaviors
{
    using System.Windows;
    using System.Windows.Documents;
    using System.Windows.Media;

    /// <summary>
    /// The multi selection mouse adorner.
    /// </summary>
    public class MultiSelectionMouseAdorner : Adorner
    {
        /// <summary>
        /// The highlight area property.
        /// </summary>
        public static readonly DependencyProperty HighlightAreaProperty = DependencyProperty.Register(
            "HighlightArea", typeof(Rect), typeof(MultiSelectionMouseAdorner), new FrameworkPropertyMetadata(new Rect(), FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// The adorned desired size.
        /// </summary>
        private Size adornedDesiredSize;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiSelectionMouseAdorner"/> class.
        /// Be sure to call the base class constructor.
        /// </summary>
        /// <param name="adornedElement">
        /// The adorned element.
        /// </param>
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

        /// <summary>
        /// Gets or sets the background brush.
        /// </summary>
        public SolidColorBrush BackgroundBrush { get; set; }

        /// <summary>
        /// Gets or sets the pen brush.
        /// </summary>
        public SolidColorBrush PenBrush { get; set; }

        /// <summary>
        /// Gets or sets the drawing pen.
        /// </summary>
        public Pen DrawingPen { get; set; }

        /// <summary>
        /// Gets or sets the highlight area update to this property will automatically trigger underlying OnRender method.
        /// </summary>
        public Rect HighlightArea
        {
            get => (Rect)this.GetValue(HighlightAreaProperty);
            set => this.SetValue(HighlightAreaProperty, value);
        }

        /// <summary>
        /// The on render.
        /// </summary>
        /// <param name="dc">
        /// The dc.
        /// </param>
        protected override void OnRender(DrawingContext dc)
        {
            dc.DrawRectangle(this.BackgroundBrush, this.DrawingPen, this.HighlightArea);
        }
    }
}
