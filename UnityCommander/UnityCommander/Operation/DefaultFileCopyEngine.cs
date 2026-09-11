
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityCommander.Abstractions;
using UnityCommander.Abstractions.Background;
using UnityCommander.Abstractions.IO;
using UnityCommander.Abstractions.Overrides;
using UnityCommander.Common.Events;
using UnityCommander.Core.IO;
using UnityCommander.Core.IO.Operations;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace UnityCommander.Operation
{
    public class DefaultFileCopyEngine : IFileCopyEngine
    {
        private readonly IOperationIndex _operationIndex;
        private readonly IFileConflictResolver _conflictResolver;
        private readonly ICopyOperationService _operationService;
        private readonly IOperationProgressService _operationProgress;
        private readonly MoveStrategyResolver _moveStrategyResolver;
        private readonly IBackgroundWorkController _backgroundWorkController;
        private readonly IEventBus _eventBus;
        private FileConflictResolutionPolicy _conflictPolicy;

        public DefaultFileCopyEngine(
            ICopyOperationService operationService,
            IOperationProgressService operationProgress,
            IOperationIndex operationIndex,
            MoveStrategyResolver moveStrategyResolver,
            IEventBus eventBus,
            IBackgroundWorkController backgroundWorkController, 
            IFileConflictResolver conflictResolver)
        {
            _eventBus = eventBus;
            _operationIndex = operationIndex;
            _operationProgress = operationProgress;
            _operationService = operationService;
            _moveStrategyResolver = moveStrategyResolver;
            _backgroundWorkController = backgroundWorkController;
            _conflictResolver = conflictResolver;
        }

        private void OnCopyFileReport(CopyInfo info)
        {
            info.Status = FileTransferStatus.Copying;
            _eventBus.Publish(this, new CopyProgressEvent(info));
        }

        private void OnFileCompleted(CopyInfo info)
        {
            _eventBus.Publish(this, new CopyCompleteEvent(info));

            //_operationIndex.Unregister(info.ItemId);
        }

        public async Task StartAsync(FileOperationRequest request)
        {
            var operation = CreateOperation(request);

            RegisterOperation(operation);

            var manager = CreateCopyManager(operation);

            try
            {
                await ExecuteOperationAsync(
                    request,
                    operation,
                    manager);
            }
            catch (OperationCanceledException)
            {
                if (request.Type == FileOperationType.Copy)
                    await CleanupAsync(operation);
                throw;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                throw;
            }
            finally
            {
                UnregisterOperation(request.OperationId);
            }
        }

        private void UnregisterOperation(Guid operationId)
        {
            _operationService.Unregister(operationId);
            _operationProgress.Unregister(operationId);
        }

        private async Task ExecuteOperationAsync(
            FileOperationRequest request,
            CopyOperation operation,
            CopyManager manager)
        {
            var deletesSourceImmediately = false;

            foreach (var item in operation.Items)
            {
                var operationContext = CreateOperationContext(
                    manager,
                    request,
                    operation,
                    item);

                var target = item.DestinationPath;

                if (item.ItemType == FileTransferItemType.Directory)
                {
                    Directory.CreateDirectory(target);
                    continue;
                };

                if (File.Exists(target) ||
                    Directory.Exists(target))
                {
                    var conflict = new FileConflict
                    {
                        SourcePath = item.SourcePath,
                        DestinationPath = target
                    };
                    if (_conflictPolicy == FileConflictResolutionPolicy.Ask)
                    {
                        var result = await _conflictResolver.ResolveAsync(
                            conflict,
                            operationContext.Cancellation.Token);

                        switch (result)
                        {
                            case FileConflictAction.Replace:
                                _eventBus.Publish(
                                this,
                                new FileStatusChangedEvent(
                                    operationContext.Info.ItemId,
                                    FileTransferStatus.Copying,
                                    operationContext.Info.Source,
                                    target));

                                break;

                            case FileConflictAction.Skip:
                                _eventBus.Publish(
                                 this,
                                 new FileStatusChangedEvent(
                                     operationContext.Info.ItemId,
                                     FileTransferStatus.Skipped,
                                     operationContext.Info.Source,
                                     target));

                                continue;

                            case FileConflictAction.ReplaceAll:
                                _conflictPolicy =
                                    FileConflictResolutionPolicy.Replace;

                                break;

                            case FileConflictAction.SkipAll:
                                _conflictPolicy =
                                    FileConflictResolutionPolicy.Skip;

                                _eventBus.Publish(
                                this,
                                new FileStatusChangedEvent(
                                    operationContext.Info.ItemId,
                                    FileTransferStatus.Skipped,
                                    operationContext.Info.Source,
                                    target));

                                continue;

                            case FileConflictAction.KeepBoth:

                                target = ResolveUniqueDestination(target);

                                item.DestinationPath = target;
                                operationContext.Info.Target = target;
                                break;

                            case FileConflictAction.Cancel:
                                _eventBus.Publish(
                                this,
                                new FileStatusChangedEvent(
                                    operationContext.Info.ItemId,
                                    FileTransferStatus.Cancelled,
                                    operationContext.Info.Source,
                                    target));

                                return;
                        }
                    }
                    else if (_conflictPolicy == FileConflictResolutionPolicy.Skip)
                    {
                        operationContext.Info.Skipped = true;
                        _eventBus.Publish(
                              this,
                              new FileStatusChangedEvent(
                                  operationContext.Info.ItemId,
                                  FileTransferStatus.Skipped,
                                  operationContext.Info.Source,
                                  target));
                        continue;
                    }
                    else if (_conflictPolicy == FileConflictResolutionPolicy.Replace)
                    {
                        _eventBus.Publish(
                              this,
                              new FileStatusChangedEvent(
                                  operationContext.Info.ItemId,
                                  FileTransferStatus.Copying,
                                  operationContext.Info.Source,
                                  target));
                    }
                }
                item.ShouldCleanupDestination = true;

                if (request.Type == FileOperationType.Copy)
                {
                    try
                    {
                        await manager.CopyAsync(
                            operationContext,
                            item.SourcePath,
                            item.DestinationPath);

                        continue;
                    }
                    catch (OperationCanceledException)
                    {
                        _eventBus.Publish(
                            this,
                            new FileStatusChangedEvent(
                                operationContext.Info.ItemId,
                                FileTransferStatus.Cancelled,
                                operationContext.Info.Source,
                                item.DestinationPath));

                        throw;
                    }
                }

                var strategy = _moveStrategyResolver.Resolve(
                    item.SourcePath,
                    item.DestinationPath);

                await strategy.ExecuteAsync(
                    operationContext,
                    item.SourcePath,
                    item.DestinationPath);

                deletesSourceImmediately |= strategy.DeletesSource;
            }

            if (request.Type == FileOperationType.Move &&
                deletesSourceImmediately)
            {
                DeleteSources(operation);
            }
        }

        private string ResolveUniqueDestination(string destination)
        {
            if (!File.Exists(destination) &&
                !Directory.Exists(destination))
            {
                return destination;
            }

            var directory = Path.GetDirectoryName(destination);

            if (string.IsNullOrEmpty(directory))
                return destination;

            var name = Path.GetFileNameWithoutExtension(destination);
            var extension = Path.GetExtension(destination);

            var index = 1;

            while (true)
            {
                var candidate = Path.Combine(
                    directory,
                    $"{name} ({index}){extension}");

                if (!File.Exists(candidate) &&
                    !Directory.Exists(candidate))
                {
                    return candidate;
                }

                index++;
            }
        }

        private OperationContext CreateOperationContext(
            CopyManager manager,
            FileOperationRequest request,
            CopyOperation operation,
            FileTransferItem item)
        {
            return new OperationContext
            {
                Manager = manager,
                OperationId = request.OperationId,
                Cancellation = new CancellationTokenSource(),
                Operation = operation,
                Info = new CopyInfo
                {
                    OperationId = request.OperationId,
                    ItemId = item.Id,
                    Source = item.SourcePath,
                    Target = item.DestinationPath,
                    DestinationPath = request.Target,
                    Status = FileTransferStatus.Pending,
                },
                BackgroundWork = _backgroundWorkController
            };
        }

        private CopyManager CreateCopyManager(CopyOperation operation)
        {
            var manager = new CopyManager(operation);

            _operationService.Register(
                manager,
                _operationProgress);

            manager.CopyFileReport += OnCopyFileReport;
            manager.FileCompleted += OnFileCompleted;

            return manager;
        }

        private void RegisterOperation(CopyOperation operation)
        {
            _operationProgress.Register(operation);

            _operationIndex.Register(
                operation,
                operation.Items
                    .SelectMany(x => new[]
                    {
                x.SourcePath,
                x.DestinationPath
                    }));
        }

        private CopyOperation CreateOperation(FileOperationRequest request)
        {
            var items = new List<FileTransferItem>();

            foreach (var source in request.Sources)
            {
                var destination = ResolveDestination(
                    request.Target,
                    source);

                AddSourceItems(
                    items,
                    source,
                    destination);
            }

            return new CopyOperation
            {
                Id = request.OperationId,
                Items = items,
                TotalBytes = items
                    .Where(x => x.ItemType == FileTransferItemType.File)
                    .Sum(x => x.Length)
            };
        }

        private void AddSourceItems(
           List<FileTransferItem> items,
           string sourcePath,
           string destinationPath)
        {
            if (File.Exists(sourcePath))
            {
                var info = new FileInfo(sourcePath);

                items.Add(new FileTransferItem
                {
                    Id = Guid.NewGuid(),
                    ItemType = FileTransferItemType.File,
                    Status = FileTransferStatus.Pending,
                    SourcePath = sourcePath,
                    DestinationPath = destinationPath,
                    Length = info.Length
                });

                return;
            }

            if (!Directory.Exists(sourcePath))
                return;

            items.Add(new FileTransferItem
            {
                Id = Guid.NewGuid(),
                ItemType = FileTransferItemType.Directory,
                Status = FileTransferStatus.Pending,
                SourcePath = sourcePath,
                DestinationPath = destinationPath,
                Length = 0
            });

            foreach (var directory in Directory.EnumerateDirectories(sourcePath))
            {
                AddSourceItems(
                    items,
                    directory,
                    Path.Combine(
                        destinationPath,
                        Path.GetFileName(directory)));
            }

            foreach (var file in Directory.EnumerateFiles(sourcePath))
            {
                AddSourceItems(
                    items,
                    file,
                    Path.Combine(
                        destinationPath,
                        Path.GetFileName(file)));
            }
        }

          private static string ResolveDestination(
            string target,
            string source)
        {
            if (File.Exists(source))
            {
                return Path.Combine(
                    target,
                    Path.GetFileName(source));
            }

            if (Directory.Exists(source))
            {
                return Path.Combine(
                    target,
                    Path.GetFileName(
                        Path.TrimEndingDirectorySeparator(source)));
            }

            return target;
        }

        private void DeleteSources(CopyOperation operation)
        {
            foreach (var item in operation.Items)
            {
                if (!item.ShouldCleanupDestination)
                    continue;

                Delete(item.SourcePath);
            }
        }

        private async Task CleanupAsync(CopyOperation operations)
        {
            foreach (var item in operations.Items)
            {
                if (!item.ShouldCleanupDestination)
                    continue;

                Delete(item.DestinationPath);
            }
        }

        private static void Delete(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
                return;
            }

            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
        }

        private static long GetSize(string path)
        {
            if (File.Exists(path))
                return new FileInfo(path).Length;

            if (Directory.Exists(path))
            {
                return Directory
                    .EnumerateFiles(path, "*", SearchOption.AllDirectories)
                    .Sum(file => new FileInfo(file).Length);
            }

            throw new FileNotFoundException("Source path not found.", path);
        }
    }
}
