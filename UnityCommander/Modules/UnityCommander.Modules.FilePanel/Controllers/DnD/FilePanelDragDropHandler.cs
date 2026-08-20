
using AvalonDock.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using UnityCommander.Abstractions.Overrides;
using UnityCommander.Common.Models.Directory;
using UnityCommander.Controls.Layout;
using UnityCommander.Modules.FilePanel.States;
using UnityCommander.Services;
using UnityCommander.Services.Interfaces;
using UnityCommander.WPF.DragDrop;

namespace UnityCommander.Modules.FilePanel.Controllers.DnD
{
    public sealed class FilePanelDragDropHandler
        : IDragDropHandler
    {
        private IFileOperationService _fileOperationService;
        private ITabActivationService _tabActivation;

        public FilePanelDragDropHandler(
            ServiceOverrideResolver overrideResolver, 
            ITabActivationService tabActivation)
        {
            _fileOperationService = overrideResolver.Resolve<IFileOperationService>();
            _tabActivation = tabActivation;
        }

        public bool CanHandle(IDropContext context)
        {
            return context is FilePanelDragDropContext;
        }

        public DragDropResult DragOver(
           IDropContext dropContext,
           DragDropContext context)
        {
            if (!HasSources(context))
                return DragDropResult.Deny();

            if (string.Equals(
                context.SourcePath,
                context.TargetPath,
                StringComparison.OrdinalIgnoreCase))
            {
                return DragDropResult.Deny();
            }

            if (!HasValidData(context.Data))
                return DragDropResult.Deny();

            if (dropContext is FilePanelDragDropContext ctx)
            {
                if (ctx.TabId is Guid tabId)
                {
                    _tabActivation.Activate(tabId);
                }
            }

            return new DragDropResult
            {
                IsAllowed = true,
                Effect = DragDropEffects.Copy,
                Adorner = ResolveAdorner(context)
            };
        }

        public Task DropAsync(
           IDropContext dropContext,
           DragDropContext context)
        {
            var sourcePaths =
                ExtractSources(context.Data);

            if (sourcePaths.Count == 0)
                return Task.CompletedTask;

            var targetPath =
                ResolveTargetPath(context)
                ?? dropContext.Target as string;

            if (string.IsNullOrWhiteSpace(targetPath))
                return Task.CompletedTask;

            _fileOperationService.CopyAsync(
                new FileOperationRequest
                {
                    Sources = sourcePaths,
                    Target = targetPath,
                    ShowDialog = true
                });

            return Task.CompletedTask;
        }

        private static List<string> ExtractSources(
            object? data)
        {
            List<string> result = new();

            switch (data)
            {
                case BaseDirectory dir:
                    result.Add(dir.Path);
                    break;

                case IList list:
                    foreach (var item in list)
                    {
                        if (item is BaseDirectory entry)
                            result.Add(entry.Path);
                    }
                    break;
            }

            return result;
        }

        private string? ResolveTargetPath(
            DragDropContext context)
        {
            switch (context.Target)
            {
                case FolderModel folder:
                    return folder.Path;

                case FileModel file:
                    return Path.GetDirectoryName(file.Path);
            }

            if (context.TargetContext is not ContentNode node)
                return null;

            return node.Context switch
            {
                FileNodeContext fileContext => fileContext.Current,
                FolderNodeContext folderContext => folderContext.Current,
                _ => null
            };
        }

        private static Type? ResolveAdorner(
            DragDropContext context)
        {
            if (context.VisualTarget is LayoutDocumentTabItem)
                return null;

            if (context.TargetContext is ContentNode node)
            {
                return node.Context is FileNodeContext
                    ? DropTargetAdorners.Insert
                    : DropTargetAdorners.Highlight;
            }

            if (context.VisualTarget is ListView)
                return DropTargetAdorners.Highlight;


            if (context.VisualTarget is Button)
                return DropTargetAdorners.Highlight;

            return null;
        }

        private bool HasSources(DragDropContext context)
        {
            return context.SourceItems.Count > 0
                || !string.IsNullOrEmpty(context.SourcePath);
        }

        private static bool HasValidData(object? data)
        {
            switch (data)
            {
                case BaseDirectory:
                    return true;

                case IList list:
                    return list.Count > 0;

                default:
                    return false;
            }
        }
    }
}
