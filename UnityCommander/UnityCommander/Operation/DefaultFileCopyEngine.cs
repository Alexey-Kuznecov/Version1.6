
using System.Threading.Tasks;
using UnityCommander.Abstractions.Overrides;

namespace UnityCommander.Operation
{
    public class DefaultFileCopyEngine : IFileCopyEngine
    {
        private CopyOperationController _controller;

        public DefaultFileCopyEngine(CopyOperationController controller)
        {
            _controller = controller;

            _controller.Completed += OnCopyCompleted;
        }

        private void OnCopyCompleted(CopyOperationResult copyOperation)
        {
            //throw new System.NotImplementedException();
        }

        public async Task StartAsync(FileOperationRequest request)
        {
            if (request != null && request.Sources.Count > 1)
            {
                // Запускаем одну общую операцию для всех источников
                await _controller?.StartCopyManyAsync(request.Sources, request.Target);
            }
            else
            {
                await _controller?.StartCopyManyAsync(new[] { request.Sources[0] }, request.Target);
            }

            await Task.CompletedTask;
        }
    }
}
