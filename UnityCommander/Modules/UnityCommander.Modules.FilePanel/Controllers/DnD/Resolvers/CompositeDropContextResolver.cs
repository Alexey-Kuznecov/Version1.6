
using System;
using System.Collections.Generic;
using System.Linq;
using UnityCommander.WPF.DragDrop;

namespace UnityCommander.Modules.FilePanel.Controllers.DnD.Resolvers
{
    public sealed class CompositeDropContextResolver
        : IDropContextResolver
    {
        private readonly IEnumerable<IDropTargetResolver> _targets;

        public CompositeDropContextResolver(
            IEnumerable<IDropTargetResolver> targets)
        {
            _targets = targets;
        }

        public bool CanResolve(DragDropContext context)
        {
            if (context.Target is not null)
                return true;

            return _targets.Any(
                x => x.CanResolve(context));
        }

        public IDropContext Resolve(
            DragDropContext context)
        {
            if (context.Target is not null)
            {
                return new FilePanelDragDropContext
                {
                    Data = context.Data,
                    Source = context.Source,
                    Target = context.Target,
                    VisualTarget = context.VisualTarget
                };
            }

            var resolver = _targets.First(
                x => x.CanResolve(context));

            var info =
                resolver.Resolve(context);

            if (info is null)
                throw new InvalidOperationException(
                    "Drop target could not be resolved.");

            return new FilePanelDragDropContext
            {
                Data = context.Data,
                Source = context.Source,
                Target = info.Path,
                VisualTarget = context.VisualTarget,
                TabId = info.TabId
            };
        }
    }
}
