
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityCommander.Abstractions.IO;
using UnityCommander.Abstractions.Overrides;
using UnityCommander.Common.Models;
using UnityCommander.Core.IO.Operations;

namespace UnityCommander.Operation
{
    public class DefaultFileCopyEngine : IFileCopyEngine
    {
        private readonly IFileStateService _stateService;
        private readonly IOperationIndex _operationIndex;

        public DefaultFileCopyEngine(
            IFileStateService stateService,
            IOperationIndex operationIndex)
        {
            _operationIndex = operationIndex;
            _stateService = stateService ?? throw new ArgumentNullException(nameof(stateService));
        }

        private void OnCopyFileReport(CopyInfo info)
        {
            var fileName = Path.GetFileName(info.Source);
            var destinationFilePath = Path.Combine(info.Destination, fileName);

            _stateService.Set(info.Id, new FileState()
            {
                SourcePath = info.Source,
                DestinationPath = destinationFilePath,
                IsCopying = true,
                RemainingTime = info.TotalTimeLeft,
                Progress = (int)Math.Round(info.TotalPercentage),
                Speed = (long)info.AverageSpeed
            });
        }

        private void OnFileCompleted(CopyInfo info)
        {
            _stateService.Remove(info.Id);
            _operationIndex.Unregister(info.Id);
        }

        public async Task StartAsync(FileOperationRequest request)
        {
            var copyManager = new CopyManager();
            var transferId = Guid.NewGuid();

            copyManager.CopyFileReport += OnCopyFileReport;
            copyManager.FileCompleted += OnFileCompleted;

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
                Items = items
            };

            _operationIndex.Register(op, items.Select(i => i.SourcePath)
                                                .Concat(items.Select(i => i.DestinationPath)));

            foreach (var item in op.Items)
            {
                var ctx = new OperationContext
                {
                    OperationId = transferId,
                    Cancellation = new CancellationTokenSource(),
                    Operation = op,
                    Info = new CopyInfo
                    {
                        Id = item.Id,
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

            await Task.CompletedTask;
        }
    }
}
