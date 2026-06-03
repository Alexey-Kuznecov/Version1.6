using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using UnityCommander.Controls.Layout;
using UnityCommander.Core.Behaviors;
using UnityCommander.Core.DragDrop;
using UnityCommander.Modules.FilePanel.States;

namespace UnityCommander.Modules.FilePanel.Controllers.DnD
{
    public class DragDropContextFactory
    {
        public DragDropContext Create(
            IDropInfo info)
        {
            object? targetContext = null;
            object? sourceContext = null;

            if (info.VisualTarget is FrameworkElement fe)
            {
                targetContext = fe.DataContext;
            }

            if (info.DragInfo.VisualSource is FrameworkElement fe2)
            {
                sourceContext = fe2.DataContext;
            }

            return new DragDropContext
            {
                Data = info.Data,
                Target = info.TargetItem,
                VisualTarget = info.VisualTarget,
                VisualSource = info.DragInfo.VisualSource,
                TargetContext = targetContext,
                SourceContext = sourceContext,
                TargetPath = GetCurrentPath(targetContext),
                SourcePath = GetCurrentPath(sourceContext),
                SourceItems =
                    ExtractSelection(sourceContext)
            };
        }

        private static IReadOnlyList<object> ExtractSelection(object? sourceContext)
        {
            if (sourceContext is ContentNode node &&
                node.Context is BaseNodeContext ctx)
            {
                return ctx.SelectedItems.Cast<object>().ToList();
            }

            return Array.Empty<object>();
        }

        private static string? GetCurrentPath(object? context)
        {
            if (context is ContentNode node &&
                node.Context is BaseNodeContext baseContext)
            {
                return baseContext.Current;
            }

            return null;
        }
    }
}