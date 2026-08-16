
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityCommander.Abstractions;
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
        private readonly IEventBus _eventBus;

        public DefaultFileCopyEngine(
            ICopyOperationService operationService,
            IOperationProgressService operationProgress,
            IOperationIndex operationIndex,
            IEventBus eventBus)
        {
            _eventBus = eventBus;
            _operationIndex = operationIndex;
            _operationProgress = operationProgress;
            _operationService = operationService;
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
            var items = request.Sources.Select(source =>
            {
                var fileName = Path.GetFileName(source);

                return new FileTransferItem
                {
                    Id = Guid.NewGuid(),
                    SourcePath = source,
                    DestinationPath = Path.Combine(request.Target, fileName)
                };
            }).ToList();

            var op = new CopyOperation
            {
                Id = request.OperationId,
                Items = items,
                TotalBytes = items.Sum(x => GetSize(x.SourcePath))
            };

            _operationProgress.Register(op);

            _operationIndex.Register(op, items.Select(i => i.SourcePath)
                .Concat(items.Select(i => i.DestinationPath)));

            var copyManager = new CopyManager(op);

            _operationService.Register(copyManager, _operationProgress);

            copyManager.CopyFileReport += OnCopyFileReport;
            copyManager.FileCompleted += OnFileCompleted;

            foreach (var item in op.Items)
            {
                var ctx = new OperationContext
                {
                    OperationId = request.OperationId,
                    Cancellation = new CancellationTokenSource(),
                    Operation = op,
                    Info = new CopyInfo
                    {
                        OperationId = request.OperationId,
                        ItemId = item.Id,
                        Source = item.SourcePath,
                        Destination = request.Target,
                    }
                };

                var srcInfo = new DirectoryInfo(item.SourcePath);
                string destForThisSource;
                if (!copyManager.CopyOnlyFolderContent && srcInfo.Exists)
                    destForThisSource = Path.Combine(request.Target, srcInfo.Name);
                else
                    destForThisSource = request.Target;

                Directory.CreateDirectory(destForThisSource);

                await copyManager.CopyAsync(ctx, item.SourcePath, destForThisSource);
            }

            _operationService.Unregister(request.OperationId);
            _operationProgress.Unregister(request.OperationId);

            await Task.CompletedTask;
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
