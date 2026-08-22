
using NLog.Targets;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using UnityCommander.WPF;
namespace UnityCommander.Modules.FilePanel.Controllers.DnD
{
    public sealed class DragHoverNavigationService
      : IDragHoverNavigationService
    {
        private readonly IProgressIndicatorService _progressIndicator;

        private readonly TimeSpan _delay;

        private CancellationTokenSource? _cancellation;

        private UIElement? _currentTarget;

        private Action? _currentAction;

        public DragHoverNavigationService(
            IProgressIndicatorService progressIndicator,
            TimeSpan? delay = null)
        {
            _delay =
                delay ?? TimeSpan.FromMilliseconds(900);

            _progressIndicator = progressIndicator;
        }

        public void Begin(
            UIElement target,
            Action action,
            bool shiftPressed)
        {
            ArgumentNullException.ThrowIfNull(target);
            ArgumentNullException.ThrowIfNull(action);

            if (ReferenceEquals(_currentTarget, target))
            {
                _currentAction = action;

                if (shiftPressed && _cancellation is null)
                    StartTimer();

                if (!shiftPressed)
                    CancelTimer();

                return;
            }

            CancelTimer();

            _currentTarget = target;
            _currentAction = action;

            if (shiftPressed)
                StartTimer();
        }

        public void Cancel()
        {
            CancelTimer();

            _currentTarget = null;
            _currentAction = null;
        }

        private void StartTimer()
        {
            CancelTimer();

            _cancellation =
                new CancellationTokenSource();

            var mode = _currentTarget is Button 
                ? ProgressIndicatorMode.Border 
                : ProgressIndicatorMode.Linear;

            _progressIndicator.Show(
                _currentTarget!, mode);

            _ = WaitForShiftHoldAsync(
                _currentAction!,
                _cancellation.Token);
        }

        private async Task WaitForShiftHoldAsync(
            Action action,
            CancellationToken token)
        {
            try
            {
                var stopwatch = Stopwatch.StartNew();

                while (stopwatch.Elapsed < _delay)
                {
                    await Task.Delay(
                        16,
                        token);

                    var progress =
                        stopwatch.Elapsed.TotalMilliseconds /
                        _delay.TotalMilliseconds;

                    _progressIndicator.Update(
                        _currentTarget,
                        Math.Min(progress, 1d));
                }

                _progressIndicator.Update(
                    _currentTarget,
                    1d);

                action();
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                _progressIndicator.Hide(_currentTarget);
            }
        }

        private void CancelTimer()
        {
            _cancellation?.Cancel();
            _cancellation?.Dispose();
            _cancellation = null;
        }
    }
}
