
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using UnityCommander.Modules.FilePanel.Controllers.DnD.Adorners;
using UnityCommander.WPF.DragDrop;

namespace UnityCommander.Modules.FilePanel.Controllers.DnD
{
    public sealed class DragDropVisualService
        : IDragDropVisualService
    {
        public void Apply(
            UIElement target,
            DragDropResult result)
        {
            DragDropVisual.SetIsDropTarget(
                target,
                result.IsAllowed);
        }

        public void Clear(UIElement target)
        {
            DragDropVisual.SetIsDropTarget(
                target,
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
