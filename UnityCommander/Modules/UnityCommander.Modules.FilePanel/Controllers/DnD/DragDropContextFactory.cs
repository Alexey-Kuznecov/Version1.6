using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using UnityCommander.Common.Models.Directory;
using UnityCommander.Controls.Layout;
using UnityCommander.Modules.FilePanel.States;
using UnityCommander.WPF.DragDrop;

namespace UnityCommander.Modules.FilePanel.Controllers.DnD
{
    public class DragDropContextFactory : IDragDropContextFactory
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
                    ExtractSelection(sourceContext),
                DropPosition = info.DropPosition,
                KeyStates = info.KeyStates
            };
        }

        private static IReadOnlyList<object> ExtractSelection(object? sourceContext)
        {
            if (sourceContext is not ContentNode node)
                return [];

            return node.Context switch
            {
                UnifiedNodeContext unified =>
                    unified.Folder.SelectedItems.Cast<object>().ToList(),

                BaseNodeContext baseContext =>
                    baseContext.SelectedItems.Cast<object>().ToList(),

                _ => []
            };
        }

        private static string? GetCurrentPath(object? context)
        {
            if (context is not ContentNode node)
                return null;

            return node.Context switch
            {
                UnifiedNodeContext unified => unified.Folder.Current,
                BaseNodeContext baseContext => baseContext.Current,
                _ => null
            };
        }
    }
}