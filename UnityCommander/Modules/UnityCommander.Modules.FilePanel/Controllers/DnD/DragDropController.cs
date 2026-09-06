
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityCommander.WPF.DragDrop;

namespace UnityCommander.Modules.FilePanel.Controllers.DnD
{
    public sealed class DragDropController : IDragDropController
    {
        private readonly IEnumerable<IDropContextResolver> _resolvers;
        private readonly IEnumerable<IDragDropHandler> _handlers;
        private readonly IDragDropVisualService _visual;
        public readonly IDragHoverNavigationService _hoverNavigationService;

        public DragDropController(
            IEnumerable<IDropContextResolver> resolvers,
            IEnumerable<IDragDropHandler> handlers,
            IDragDropVisualService visual,
            IDragHoverNavigationService hoverNavigationService)
        {
            _resolvers = resolvers;
            _handlers = handlers;
            _visual = visual;
            _hoverNavigationService = hoverNavigationService;
        }

        public DragDropResult DragOver(
            DragDropContext context)
        {
            var resolver =
                _resolvers.FirstOrDefault(
                    r => r.CanResolve(context));

            if (resolver == null)
            {
                _hoverNavigationService.Cancel();
                return DragDropResult.Deny();
            }

            var dropContext =
                resolver.Resolve(context);

            if (dropContext == null)
            {
                _hoverNavigationService.Cancel();
                return DragDropResult.Deny();
            }

            var handler =
                _handlers.FirstOrDefault(
                    h => h.CanHandle(dropContext));

            if (handler == null)
                return DragDropResult.Deny();

            var result =
                handler.DragOver(
                    dropContext,
                    context);
                
            _visual.Apply(
                context.VisualTarget,
                result);

            return result;
        }

        public Task DropAsync(
            DragDropContext context)
        {
            var resolver =
                _resolvers.FirstOrDefault(
                    r => r.CanResolve(context));

            if (resolver == null)
                return Task.CompletedTask;

            var dropContext =
                resolver.Resolve(context);

            var handler =
                _handlers.FirstOrDefault(
                    h => h.CanHandle(dropContext));

            if (handler == null)
                return Task.CompletedTask;

            _visual.Clear(
               context.VisualTarget);

            return handler.DropAsync(
                dropContext,
                context);
        }

        public void DragLeave(
            DragDropContext context)
        {
            var resolver =
                    _resolvers.FirstOrDefault(
                        r => r.CanResolve(context));

            if (resolver == null)
                return;

            var dropContext =
              resolver.Resolve(context);

            var handler =
                _handlers.FirstOrDefault(
                    h => h.CanHandle(dropContext));

            handler.DragLeave(
                dropContext,
                context);

            _visual.Clear(
                context.VisualTarget);
        }

        public void DragEnter(DragDropContext context)
        {
        }
    }
}
