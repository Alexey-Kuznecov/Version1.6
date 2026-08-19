
using AvalonDock.Controls;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityCommander.WPF.DragDrop;

namespace UnityCommander.Modules.FilePanel.Controllers.DnD
{
    public class TabDragActivationService : ITabDragActivationService
    {
        private LayoutDocumentTabItem? _pendingTab;
        private CancellationTokenSource? _activationCts;

        public void DragLeave()
        {
            Cancel();
        }

        public void DragOver(DragDropContext context)
        {
            if (context.VisualTarget is not LayoutDocumentTabItem tab)
            {
                Cancel();
                return;
            }

            //if (tab.IsSelected)
            //{
            //    Cancel();
            //    return;
            //}

            if (ReferenceEquals(_pendingTab, tab))
                return;

            Cancel();

            _pendingTab = tab;
            _activationCts = new CancellationTokenSource();

            _ = ActivateAfterDelayAsync(
                tab,
                _activationCts.Token);
        }

        private void Cancel()
        {
            _activationCts?.Cancel();
            _activationCts?.Dispose();
            _activationCts = null;
            _pendingTab = null;
        }

        private async Task ActivateAfterDelayAsync(
            LayoutDocumentTabItem tab,
            CancellationToken token)
        {
            try
            {
                await Task.Delay(
                    TimeSpan.FromMilliseconds(700),
                    token);

                if (token.IsCancellationRequested)
                    return;

                tab.Model.IsSelected = true;
            }
            catch (OperationCanceledException)
            {
            }
        }
    }
}
