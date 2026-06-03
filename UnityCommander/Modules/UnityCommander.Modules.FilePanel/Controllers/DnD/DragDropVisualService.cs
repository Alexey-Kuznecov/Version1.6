
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using UnityCommander.Core.Behaviors;
using UnityCommander.Modules.FilePanel.Controllers.DnD.Adorders;

namespace UnityCommander.Modules.FilePanel.Controllers.DnD
{
    public sealed class DragDropVisualService
        : IDragDropVisualService
    {
        public void Apply(
            UIElement target,
            DragDropResult result)
        {
            var adorner = AdornerLayer.GetAdornerLayer(target);
            if (adorner == null)
                this.CreateAdornerLayer(target);

            var border = FindParent<Border>(target);

            if (border != null)
            {
                DragDropVisual.SetIsDropTarget(border, result.IsAllowed);
                var value = DragDropVisual.GetIsDropTarget(border);
                if (value)
                {
                    Debug.WriteLine(value.ToString());
                }
            }
        }

        public void Clear(UIElement target)
        {
            var border = FindParent<Border>(target);
            DragDropVisual.SetIsDropTarget(
                border,
                false);
        }

        private void CreateAdornerLayer(UIElement element)
        {
            if (element is ListBox listBox &&
                listBox.Parent is Border border)
            {
                border.Child = null;

                var decorator = new AdornerDecorator
                {
                    Child = listBox
                };

                border.Child = decorator;
            }
        }

        private static T FindParent<T>(DependencyObject child)
            where T : DependencyObject
        {
            DependencyObject current = child;

            while (current != null)
            {
                if (current is T result)
                    return result;

                current = VisualTreeHelper.GetParent(current);
            }

            return null;
        }
    }
}
