
using System;
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
using UnityCommander.Core.Events;
using UnityCommander.Core.IO;
using UnityCommander.Core.IO.Operations;

namespace UnityCommander.Operation
{
    public class DefaultFileCopyEngine : IFileCopyEngine
    {
        private readonly IOperationIndex _operationIndex;
        private readonly ICopyOperationService _operationService;
        private readonly IOperationProgressService _operationProgress;
        private readonly MoveStrategyResolver _moveStrategyResolver;
        private readonly IBackgroundWorkController _backgroundWorkController;
        private readonly IEventBus _eventBus;

        public DefaultFileCopyEngine(
            ICopyOperationService operationService,
            IOperationProgressService operationProgress,
            IOperationIndex operationIndex,
            MoveStrategyResolver moveStrategyResolver,
            IEventBus eventBus,
            IBackgroundWorkController backgroundWorkController)
        {
            _eventBus = eventBus;
            _operationIndex = operationIndex;
            _operationProgress = operationProgress;
            _operationService = operationService;
            _moveStrategyResolver = moveStrategyResolver;
            _backgroundWorkController = backgroundWorkController;
        }

        private void OnCopyFileReport(CopyInfo info)
        {
            _eventBus.Publish(this, new CopyProgressEvent(info));
        }

        private void OnFileCompleted(CopyInfo info)
        {
            _eventBus.Publish(this, new CopyCompleteEvent(info));
            _operationIndex.Unregister(info.ItemId);
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
                //else
                //    DeleteSources(operation);
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
                var context = CreateOperationContext(
                    manager,
                    request,
                    operation,
                    item);

                var destination = ResolveDestination(
                    request.Target,
                    item.SourcePath,
                    manager);

                //Directory.CreateDirectory(destination);

                item.ShouldCleanupDestination = true;

                if (request.Type == FileOperationType.Copy)
                {
                    await manager.CopyAsync(
                        context,
                        item.SourcePath,
                        destination);

                    continue;
                }

                var strategy = _moveStrategyResolver.Resolve(
                    item.SourcePath,
                    destination);

                await strategy.ExecuteAsync(
                    context,
                    item.SourcePath,
                    destination);
                
                deletesSourceImmediately |= strategy.DeletesSource;
            }

            if (request.Type == FileOperationType.Move &&
                deletesSourceImmediately)
            {
                DeleteSources(operation);
            }
        }

        private string ResolveDestination(
            string target,
            string source,
            CopyManager copyManager)
        {
            var sourceInfo = new DirectoryInfo(source);

            if (!copyManager.CopyOnlyFolderContent &&
                sourceInfo.Exists)
            {
                return Path.Combine(
                    target,
                    sourceInfo.Name);
            }

            return target;
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
                    Destination = request.Target
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
            var items = request.Sources
                .Select(source => new FileTransferItem
                {
                    Id = Guid.NewGuid(),
                    Status = FileTransferStatus.Pending,
                    SourcePath = source,
                    DestinationPath = Path.Combine(
                        request.Target,
                        Path.GetFileName(source))
                })
                .ToList();

            return new CopyOperation
            {
                Id = request.OperationId,
                Items = items,
                TotalBytes = items.Sum(x => GetSize(x.SourcePath))
            };
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
