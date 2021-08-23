
namespace UnityCommander.Modules.FilePanel.Behaviors.DragDrop
{
    using System;
    using System.Windows;
    using System.Windows.Documents;
    using System.Windows.Media;

    /// <summary>
    /// The drag adorner.
    /// </summary>
    public class DragAdorner : Adorner
    {
        /// <summary>
        /// The m adorner layer.
        /// </summary>
        private readonly AdornerLayer adornerLayer;

        /// <summary>
        /// The m_ adornment.
        /// </summary>
        private readonly UIElement adornment;

        /// <summary>
        /// The m_ mouse position.
        /// </summary>
        private Point mousePosition;

        /// <summary>
        /// The translation.
        /// </summary>
        private Point translation;

        /// <summary>
        /// Initializes a new instance of the <see cref="DragAdorner"/> class.
        /// </summary>
        /// <param name="adornedElement">
        /// The adorned element.
        /// </param>
        /// <param name="adornment">
        /// The adornment.
        /// </param>
        /// <param name="translation">
        /// The translation.
        /// </param>
        /// <param name="effects">
        /// The effects.
        /// </param>
        public DragAdorner(UIElement adornedElement, UIElement adornment, Point translation, DragDropEffects effects = DragDropEffects.None)
            : base(adornedElement)
        {
            this.translation = translation;
            this.adornerLayer = AdornerLayer.GetAdornerLayer(adornedElement);
            this.adornerLayer?.Add(this);
            this.adornment = adornment;
            this.IsHitTestVisible = false;
            this.Effects = effects;
        }
        
        /// <summary>
        /// Gets the effects.
        /// </summary>
        public DragDropEffects Effects { get; }

        /// <summary>
        /// Gets or sets the mouse position.
        /// </summary>
        public Point MousePosition
        {
            get => this.mousePosition;
            set
            {
                if (this.mousePosition != value)
                {
                    this.mousePosition = value;
                    this.adornerLayer.Update(this.AdornedElement);
                }
            }
        }

        /// <summary>
        /// Gets the visual children count.
        /// </summary>
        protected override int VisualChildrenCount => 1;

        /// <summary>
        /// The detatch.
        /// </summary>
        public void Detatch()
        {
            this.adornerLayer.Remove(this);
        }

        /// <summary>
        /// The get desired transform.
        /// </summary>
        /// <param name="transform">
        /// The transform.
        /// </param>
        /// <returns>
        /// The <see cref="GeneralTransform"/>.
        /// </returns>
        public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
        {
            var result = new GeneralTransformGroup();
            result.Children.Add(base.GetDesiredTransform(transform) ?? throw new InvalidOperationException());
            result.Children.Add(new TranslateTransform(this.MousePosition.X + this.translation.X, this.MousePosition.Y + this.translation.Y));

            return result;
        }

        /// <summary>
        /// A common way to implement an adorner rendering behavior is to override the OnRender
        /// method, which is called by the layout system as part of a rendering pass.
        /// </summary>
        /// <param name="drawingContext">
        /// The drawing context.
        /// </param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            Rect adornedElementRect = new Rect(this.AdornedElement.DesiredSize);

            // Some arbitrary drawing implements.
            SolidColorBrush renderBrush = new SolidColorBrush(Colors.Green);
            renderBrush.Opacity = 0.2;
            Pen renderPen = new Pen(new SolidColorBrush(Colors.Navy), 1.5);
            double renderRadius = 5.0;

            // Draw a circle at each corner.
            drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.TopLeft, renderRadius, renderRadius);
            drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.TopRight, renderRadius, renderRadius);
            drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.BottomLeft, renderRadius, renderRadius);
            drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.BottomRight, renderRadius, renderRadius);
        }

        /// <summary>
        /// The arrange override.
        /// </summary>
        /// <param name="finalSize">
        /// The final size.
        /// </param>
        /// <returns>
        /// The <see cref="Size"/>.
        /// </returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            this.adornment.Arrange(new Rect(finalSize));
            return finalSize;
        }

        /// <summary>
        /// The get visual child.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="Visual"/>.
        /// </returns>
        protected override Visual GetVisualChild(int index) => this.adornment;

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
            this.adornment.Measure(constraint);
            return this.adornment.DesiredSize;
        }
    }
}
