
using System.Windows;
using System.Windows.Documents;

namespace UnityCommander.WPF
{
    public class ProgressIndicatorService : IProgressIndicatorService
    {
        private readonly Dictionary<UIElement, ProgressAdorner> _adorners = new();

        public void Show(UIElement target,
           ProgressIndicatorMode mode)
        {
            if (_adorners.ContainsKey(target))
                return;

            var layer = AdornerLayer.GetAdornerLayer(target);

            if (layer is null)
                return;

            var adorner = new ProgressAdorner(target, mode);

            _adorners[target] = adorner;
            layer.Add(adorner);
        }

        public void Update(
            UIElement target,
            double progress)
        {
            if (!_adorners.TryGetValue(
                    target,
                    out var adorner))
            {
                return;
            }

            adorner.Progress = progress;
        }

        public void Hide(UIElement target)
        {
            if (!_adorners.Remove(
                    target,
                    out var adorner))
            {
                return;
            }

            var layer = AdornerLayer.GetAdornerLayer(target);

            layer?.Remove(adorner);
        }
    }
}
