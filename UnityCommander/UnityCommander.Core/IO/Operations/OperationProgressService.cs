
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityCommander.Abstractions;
using UnityCommander.Abstractions.IO;
using UnityCommander.Common.Events;
using UnityCommander.Core.Events;

namespace UnityCommander.Core.IO.Operations
{
    public sealed class OperationProgressService : IOperationProgressService
    {
        private readonly ConcurrentDictionary<Guid, OperationRuntime> _operations = new();

        private int _activeOperations;

        public event Action<OperationState>? ProgressChanged;

        public event Action<OperationState>? OperationCompleted;

        public event Action? AllOperationsCompleted;

        public OperationProgressService(IEventBus eventBus)
        {
            eventBus.Subscribe<CopyProgressEvent>(OnProgress);
            eventBus.Subscribe<CopyCompleteEvent>(OnComplete);
        }

        public void Register(CopyOperation operation)
        {
            if (operation == null)
                throw new ArgumentNullException(nameof(operation));

            var runtime = new OperationRuntime(
                new OperationState
                {
                    OperationId = operation.Id,
                    TotalBytes = operation.TotalBytes,
                    TotalItems = operation.Items.Count,
                    CompletedBytes = 0,
                    CompletedItems = 0,
                    Speed = 0,
                    Status = OperationStatus.InProgress
                });

            if (!_operations.TryAdd(operation.Id, runtime))
                throw new InvalidOperationException(
                    $"Operation '{operation.Id}' is already registered.");

            Interlocked.Increment(ref _activeOperations);
        }

        public void Unregister(Guid operationId)
        {
            if (!_operations.TryRemove(operationId, out _))
                return;

            if (Interlocked.Decrement(ref _activeOperations) == 0)
            {
                AllOperationsCompleted?.Invoke();
            }
        }

        public OperationState? Get(Guid operationId)
        {
            return _operations.TryGetValue(operationId, out var runtime)
                ? runtime.State
                : null;
        }

        public IReadOnlyCollection<OperationState> GetAll()
        {
            return _operations.Values
                .Select(x => x.State)
                .ToList();
        }

        public OperationState? GetGlobalState()
        {
            var states = _operations.Values
                .Select(x => x.State)
                .ToList();

            if (states.Count == 0)
                return null;

            var totalBytes = states.Sum(x => x.TotalBytes);
            var completedBytes = states.Sum(x => x.CompletedBytes);

            var totalItems = states.Sum(x => x.TotalItems);
            var completedItems = states.Sum(x => x.CompletedItems);

            return new OperationState
            {
                TotalBytes = totalBytes,
                CompletedBytes = completedBytes,
                TotalItems = totalItems,
                CompletedItems = completedItems,
                Speed = states.Sum(x => x.Speed),
                Status = states.All(x => x.Status == OperationStatus.Completed)
                    ? OperationStatus.Completed
                    : OperationStatus.InProgress
            };
        }

        private void OnProgress(object? sender, CopyProgressEvent e)
        {
            var info = e.Info;

            if (!_operations.TryGetValue(info.OperationId, out var runtime))
                return;

            // Прогресс конкретного файла.
            runtime.ItemCompletedBytes[info.ItemId] =
                (long)info.TotalByteDone;

            // А теперь собираем прогресс всей операции.
            runtime.State.CompletedBytes =
                runtime.ItemCompletedBytes.Values.Sum();

            runtime.State.Speed = (long)info.AverageSpeed;
            runtime.State.Status = OperationStatus.InProgress;

            ProgressChanged?.Invoke(runtime.State);
        }

        private void OnComplete(object? sender, CopyCompleteEvent e)
        {
            if (!_operations.TryGetValue(e.Info.OperationId, out var runtime))
                return;

            // Этот конкретный файл теперь полностью скопирован.
            runtime.ItemCompletedBytes[e.Info.ItemId] =
                GetItemTotalBytes(e.Info);

            // Чтобы один файл не посчитался дважды.
            runtime.CompletedItems.TryAdd(e.Info.ItemId, 0);

            runtime.State.CompletedBytes =
                runtime.ItemCompletedBytes.Values.Sum();

            runtime.State.CompletedItems =
                runtime.CompletedItems.Count;

            // Скорость оставляем от текущего события.
            runtime.State.Speed = (long)e.Info.AverageSpeed;

            if (runtime.State.CompletedItems >= runtime.State.TotalItems)
            {
                // Последний файл завершён.
                runtime.State.CompletedBytes =
                    runtime.State.TotalBytes;

                runtime.State.Speed = 0;
                runtime.State.Status = OperationStatus.Completed;
            }
            else
            {
                runtime.State.Status = OperationStatus.InProgress;
            }

            ProgressChanged?.Invoke(runtime.State);

            if (runtime.State.CompletedItems >= runtime.State.TotalItems)
            {
                runtime.State.CompletedBytes = runtime.State.TotalBytes;
                runtime.State.Speed = 0;
                runtime.State.Status = OperationStatus.Completed;

                OperationCompleted?.Invoke(runtime.State);
            }
        }

        private static long GetItemTotalBytes(CopyInfo info)
        {
            return (long)info.TotalBytes;
        }
    }
}
