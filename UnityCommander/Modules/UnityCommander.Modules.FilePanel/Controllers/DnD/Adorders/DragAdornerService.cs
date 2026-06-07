
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using UnityCommander.Core;

namespace UnityCommander.Modules.FilePanel.Controllers.DnD.Adorders
{
    public class DragAdornerService
    {
        private AdornerLayer _layer;
        private DragAdorner _adorner;

        public void Show(UIElement element)
        {
            _layer = AdornerLayer.GetAdornerLayer(element);

            if (_layer == null)
                return;

            var template = ResourceManager.Get<DataTemplate>("GhostFileTemplate");
            _adorner = new DragAdorner(element, null, template);
            _layer.Add(_adorner);
        }

        public void UpdatePosition(Point position)
        {
            if (_adorner == null)
                return;

            _adorner.RenderTransform = new TranslateTransform(
                position.X,
                position.Y);
        }

        public void Remove()
        {
            if (_layer != null && _adorner != null)
            {
                _layer.Remove(_adorner);
            }
        }
    }
}
