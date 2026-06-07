
using Prism.Dialogs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using UnityCommander.Common.Models.Directory;
using UnityCommander.Controls.Layout;
using UnityCommander.Core.Behaviors;
using UnityCommander.Core.DragDrop;
using UnityCommander.Core.Mvvm;
using UnityCommander.Modules.FilePanel.States;

namespace UnityCommander.Modules.FilePanel.Controllers.DnD
{
    public sealed class FilePanelDragDropHandler
     : IDragDropHandler
    {
        private readonly IDialogService _dialogService;

        public FilePanelDragDropHandler(IDialogService dialogService)
        {
            _dialogService = dialogService;
        }

        public bool CanHandle(IDropContext context)
        {
            return context is FilePanelDragDropContext;
        }

        public DragDropResult DragOver(
           IDropContext dropContext,
           DragDropContext context)
        {
            if (context.SourceItems.Count == 0)
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

            var cxt = ((ContentNode)context.TargetContext).Context is FileNodeContext;

            return new DragDropResult
            {
                IsAllowed = true,
                Effect = DragDropEffects.Copy,
                Adorner = cxt ? DropTargetAdorners.Insert : DropTargetAdorners.Highlight,
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
                ResolveTargetPath(context);

            if (string.IsNullOrWhiteSpace(targetPath))
                return Task.CompletedTask;

            _dialogService.ShowDialog(
                "CopyDialog",
                new OverrideDialogParameters(
                    new CopyParameters
                    {
                        ManySource = sourcePaths,
                        Target = targetPath
                    }),
                _ => { });

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
