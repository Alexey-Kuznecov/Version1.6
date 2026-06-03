using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace UnityCommander.Core.DragDrop
{
    public abstract class DropTargetAdorner : Adorner
    {
        [Obsolete("This constructor is obsolete and will be deleted in next major release.")]
        public DropTargetAdorner(UIElement adornedElement)
            : this(adornedElement, (DropInfo)null)
        {
        }

        public DropTargetAdorner(UIElement adornedElement, DropInfo dropInfo)
            : base(adornedElement)
        {
            this.DropInfo = dropInfo;
            this.IsHitTestVisible = false;
            this.AllowDrop = false;
            this.SnapsToDevicePixels = true;

            Debug.WriteLine(dropInfo.VisualTarget?.GetType());
            Debug.WriteLine(dropInfo.VisualTargetItem?.GetType());
            Debug.WriteLine(dropInfo.TargetItem?.GetType());
            this.m_AdornerLayer = AdornerLayer.GetAdornerLayer(adornedElement);
            this.m_AdornerLayer.Add(this);
        }

        public DropInfo DropInfo { get; set; }

        /// <summary>
        /// Gets or Sets the pen which can be used for the render process.
        /// </summary>
        public Pen Pen { get; set; } = new Pen(Brushes.Gray, 2);

        public void Detatch()
        {
            this.m_AdornerLayer.Remove(this);
        }

        internal static DropTargetAdorner Create(Type type, UIElement adornedElement, IDropInfo dropInfo)
        {
            if (!typeof(DropTargetAdorner).IsAssignableFrom(type))
            {
                throw new InvalidOperationException("The requested adorner class does not derive from DropTargetAdorner.");
            }
            return type.GetConstructor(new[] { typeof(UIElement), typeof(DropInfo) })?.Invoke(new object[] { adornedElement, dropInfo }) as DropTargetAdorner;
        }

        private readonly AdornerLayer m_AdornerLayer;

        private void CreateAdornerLayer(UIElement element)
        {
            if (element is ListBox listBox && listBox.Parent is Grid parent)
            {
                parent.Children.Remove(listBox);
                var decorator = new AdornerDecorator { Child = listBox };
                parent.Children.Add(decorator);
            }
        }
    }
}