
using System.Diagnostics;

namespace UnityCommander.WPF.DragDrop
{
    public sealed class GongDropAdapter : IDropTarget
    {
        private readonly IDragDropController _controller;
        private readonly IDragDropContextFactory _factory;

        public GongDropAdapter(IDragDropController controller, IDragDropContextFactory factory)
        {
            _controller = controller;
            _factory = factory;
        }

        public void DragOver(IDropInfo dropInfo)
        {
            Debug.WriteLine(
               $"DRAG OVER: " +
               $"Target={dropInfo.TargetItem?.GetType().FullName}, " +
               $"VisualTarget={dropInfo.VisualTarget?.GetType().FullName}, " +
               $"Source={dropInfo.DragInfo?.VisualSource?.GetType().FullName}");

            var context = _factory.Create(dropInfo);

            var result = _controller.DragOver(context);

            dropInfo.Effects = result.Effect;
            dropInfo.DropTargetAdorner = result.Adorner;
        }

        public void DragEnter(IDropInfo dropInfo)
        {
            var context =
               _factory.Create(dropInfo);

            _controller.DragEnter(context);
        }

        public void DragLeave(IDropInfo dropInfo)
        {
            var context =
                _factory.Create(dropInfo);

            _controller.DragLeave(context);
        }

        public async void Drop(IDropInfo dropInfo)
        {
            var context = _factory.Create(dropInfo);

            await _controller.DropAsync(context);
        }
    }
}
