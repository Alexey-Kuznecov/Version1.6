
using AvalonDock.Controls;
using NLog.Targets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UnityCommander.Abstractions.Keyboard;
using UnityCommander.Abstractions.Overrides;
using UnityCommander.Abstractions.Panels;
using UnityCommander.Common.Models.Directory;
using UnityCommander.Controls.Layout;
using UnityCommander.Modules.FilePanel.States;
using UnityCommander.Services;
using UnityCommander.Services.Interfaces;
using UnityCommander.WPF;
using UnityCommander.WPF.DragDrop;
using UnityCommander.WPF.Input;

namespace UnityCommander.Modules.FilePanel.Controllers.DnD
{
    public sealed class FilePanelDragDropHandler
        : IDragDropHandler
    {
        private readonly IFileOperationService _fileOperationService;
        
        private readonly ITabActivationService _tabActivation;

        public readonly IDragHoverNavigationService _hoverNavigationService;
        
        private readonly ICursorTargetService _cursorTargetsService;

        private readonly IInputState _inputState;

        public FilePanelDragDropHandler(
            ServiceOverrideResolver overrideResolver, 
            ITabActivationService tabActivation, 
            IDragHoverNavigationService hoverNavigationService, 
            IInputState inputState, 
            ICursorTargetService cursorTarget)
        {
            _fileOperationService = overrideResolver.Resolve<IFileOperationService>();
            _tabActivation = tabActivation;
            _hoverNavigationService = hoverNavigationService;
            _inputState = inputState;
            _cursorTargetsService = cursorTarget;
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

            if (IsInvalidDrop(context))
                return DragDropResult.Deny();

            if (!HasValidData(context.Data))
                return DragDropResult.Deny();

            if (dropContext is FilePanelDragDropContext ctx)
            {
                if (ctx.TabId is Guid tabId)
                {
                    _tabActivation.Activate(tabId);
                }

                if (ctx.CanNavigate)
                {
                    var shiftPressed =
                        (context.KeyStates & DragDropKeyStates.ShiftKey) != 0;

                    if (context.VisualTarget is ListView listView &&
                        context.DropPosition is Point position)
                    {
                        _cursorTargetsService.Update(
                            listView,
                            position);

                        var cursorTarget =
                            _cursorTargetsService.GetCurrent(listView);

                        if (cursorTarget?.Element.Content is IFolderItem folder &&
                            ctx.TargetInfo?.NavigateCommand is ICommand navCommand)
                        {
                            _hoverNavigationService.Begin(
                                cursorTarget.Element,
                                () => navCommand.Execute(folder),
                                shiftPressed);
                        }
                        else
                        {
                            _hoverNavigationService.Cancel();
                        }
                    }

                    if (context.VisualTarget is Button button &&
                        ctx.TargetInfo?.NavigateCommand is ICommand command &&
                        button.CommandParameter is string path)
                    {
                        _hoverNavigationService.Begin(
                            button,
                            () => command.Execute(path),
                            shiftPressed);
                    }
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

        public void DragLeave(
            IDropContext dropContext,
            DragDropContext context)
        {
            if (context.VisualTarget is ListView listView)
            {
                _cursorTargetsService.Clear(listView);
            }

            _hoverNavigationService.Cancel();
        }

        private static bool IsInvalidDrop(DragDropContext context)
        {
            if (string.IsNullOrEmpty(context.TargetPath))
                return false;

            foreach (var item in context.SourceItems)
            {
                if (item is not BaseDirectory source)
                    continue;

                var sourcePath = source.Path;

                if (string.Equals(
                        sourcePath,
                        context.TargetPath,
                        StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                if (context.TargetPath.StartsWith(
                        sourcePath.TrimEnd(Path.DirectorySeparatorChar)
                        + Path.DirectorySeparatorChar,
                        StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
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
