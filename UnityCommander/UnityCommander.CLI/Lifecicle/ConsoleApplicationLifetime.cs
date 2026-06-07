
namespace UnityCommander.CLI.Lifecicle
{
    public sealed class ConsoleApplicationLifetime
    {
        private bool _isRunning = true;
        private CommandProcessManager _processManager;

        private readonly CancellationTokenSource _cts = new();

        public bool IsRunning => _isRunning;

        public CancellationToken Token => _cts.Token;

        public event Action? ApplicationStopping;
        public event Action? ApplicationStopped;

        public ConsoleApplicationLifetime(CommandProcessManager processManager)
        {
            _processManager = processManager;
        }

        public void Stop()
        {
            if (!_isRunning)
                return;

            _isRunning = false;

            ApplicationStopping?.Invoke();

            _cts.Cancel();
            _processManager.StopAll();
        }

        public void NotifyStopped()
        {
            ApplicationStopped?.Invoke();
        }
    }
}
