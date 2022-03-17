using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Components.DragDrop
{
    public abstract class DropTargetAdorner : Adorner
    {
        public DropTargetAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            this.IsHitTestVisible = false;
            this.AllowDrop = false;
            this.SnapsToDevicePixels = true;
            this.adornerLayer = AdornerLayer.GetAdornerLayer(adornedElement);
            this.adornerLayer?.Add(this);
        }

        /// <summary>
        /// Gets or Sets the pen which can be used for the render process.
        /// </summary>
        public Pen Pen { get; set; } = new Pen(Brushes.Gray, 2);

        public void Detatch()
        {
            this.adornerLayer.Remove(this);
        }

        internal static DropTargetAdorner Create(Type type, UIElement adornedElement)
        {
            if (!typeof(DropTargetAdorner).IsAssignableFrom(type))
            {
                throw new InvalidOperationException("The requested adorner class does not derive from DropTargetAdorner.");
            }

            return
                null; // type.GetConstructor(new[] { typeof(UIElement), typeof(DropInfo) })?.Invoke(new object[] { adornedElement, dropInfo }) as DropTargetAdorner;
        }

        private readonly AdornerLayer adornerLayer;
    }
}