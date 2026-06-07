
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityCommander.Core.Behaviors;

namespace UnityCommander.Modules.FilePanel.Controllers.DnD
{
    public sealed class DragDropController
    {
        private readonly IEnumerable<IDropContextResolver> _resolvers;
        private readonly IEnumerable<IDragDropHandler> _handlers;
        private readonly IDragDropVisualService _visual;

        public DragDropController(
            IEnumerable<IDropContextResolver> resolvers,
            IEnumerable<IDragDropHandler> handlers,
            IDragDropVisualService visual)
        {
            _resolvers = resolvers;
            _handlers = handlers;
            _visual = visual;
        }

        public DragDropResult DragOver(
            DragDropContext context)
        {

            var resolver =
                _resolvers.FirstOrDefault(
                    r => r.CanResolve(context));

            if (resolver == null)
                return DragDropResult.Deny();

            var dropContext =
                resolver.Resolve(context);

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

            return handler.DropAsync(
                dropContext,
                context);
        }

        public void DragLeave(
            DragDropContext context)
        {
            _visual.Clear(
                context.VisualTarget);
        }

        public void DragEnter(DragDropContext context)
        {
        }
    }
}
