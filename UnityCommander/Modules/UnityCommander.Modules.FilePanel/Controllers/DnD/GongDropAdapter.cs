using UnityCommander.Core.DragDrop;

namespace UnityCommander.Modules.FilePanel.Controllers.DnD
{
    public sealed class GongDropAdapter : IDropTarget
    {
        private readonly DragDropController _controller;
        private readonly DragDropContextFactory _factory;

        public GongDropAdapter(DragDropController controller, DragDropContextFactory factory)
        {
            _controller = controller;
            _factory = factory;
        }

        public void DragOver(IDropInfo dropInfo)
        {
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
