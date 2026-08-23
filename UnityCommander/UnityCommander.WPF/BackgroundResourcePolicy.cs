
using UnityCommander.Abstractions.Background;

namespace UnityCommander.WPF
{
    public sealed class BackgroundResourcePolicy
      : IBackgroundResourcePolicy,
        IDisposable
    {
        private readonly IUserActivityService _activity;

        public BackgroundPriority Priority { get; private set; }

        public event EventHandler<BackgroundPriority>? PriorityChanged;

        public BackgroundResourcePolicy(
            IUserActivityService activity)
        {
            _activity = activity;

            Priority =
                activity.State == UserActivityState.Active
                    ? BackgroundPriority.Low
                    : BackgroundPriority.Normal;

            _activity.StateChanged +=
                OnActivityChanged;
        }

        private void OnActivityChanged(
            object? sender,
            UserActivityState state)
        {
            var priority =
                state == UserActivityState.Active
                    ? BackgroundPriority.Low
                    : BackgroundPriority.Normal;

            if (Priority == priority)
                return;

            Priority = priority;

            PriorityChanged?.Invoke(
                this,
                priority);
        }

        public void Dispose()
        {
            _activity.StateChanged -=
                OnActivityChanged;
        }
    }
}
