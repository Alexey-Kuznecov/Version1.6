
using System.Windows.Input;
using System.Windows.Threading;
using UnityCommander.Abstractions.Background;

namespace UnityCommander.WPF
{
    public sealed class UserActivityService
     : IUserActivityService,
       IDisposable
    {
        private readonly DispatcherTimer _timer;

        private DateTime _lastActivityUtc;
        private UserActivityState _state;

        public UserActivityState State => _state;

        public TimeSpan IdleTimeout { get; }

        public event EventHandler<UserActivityState>? StateChanged;
        public event EventHandler? Activity;

        public UserActivityService(
            TimeSpan? idleTimeout = null)
        {
            IdleTimeout =
                idleTimeout ?? TimeSpan.FromSeconds(2);

            _lastActivityUtc = DateTime.UtcNow;
            _state = UserActivityState.Active;

            InputManager.Current.PreProcessInput +=
                OnPreProcessInput;

            _timer = new DispatcherTimer(
                TimeSpan.FromMilliseconds(250),
                DispatcherPriority.ApplicationIdle,
                OnTimer,
                Dispatcher.CurrentDispatcher);

            _timer.Start();
        }

        public void NotifyActivity()
        {
            _lastActivityUtc = DateTime.UtcNow;

            Activity?.Invoke(
                this,
                EventArgs.Empty);

            SetState(UserActivityState.Active);
        }

        private void OnPreProcessInput(
            object? sender,
            PreProcessInputEventArgs e)
        {
            if (!IsUserInput(e.StagingItem.Input))
                return;

            NotifyActivity();
        }

        private void OnTimer(
            object? sender,
            EventArgs e)
        {
            if (DateTime.UtcNow - _lastActivityUtc <
                IdleTimeout)
            {
                return;
            }

            SetState(UserActivityState.Idle);
        }

        private void SetState(
            UserActivityState state)
        {
            if (_state == state)
                return;

            _state = state;

            StateChanged?.Invoke(
                this,
                state);
        }

        private static bool IsUserInput(
            InputEventArgs input)
        {
            return input switch
            {
                KeyboardEventArgs => true,
                MouseButtonEventArgs => true,
                MouseWheelEventArgs => true,

                // MouseMove отдельно можно подключить
                // позже, если действительно понадобится.
                _ => false
            };
        }

        public void Dispose()
        {
            InputManager.Current.PreProcessInput -=
                OnPreProcessInput;

            _timer.Stop();
        }
    }
}
