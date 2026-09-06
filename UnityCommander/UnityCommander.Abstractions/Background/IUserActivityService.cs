
namespace UnityCommander.Abstractions.Background
{
    public enum UserActivityState
    {
        Idle,
        Active
    }

    public interface IUserActivityService
    {
        UserActivityState State { get; }

        TimeSpan IdleTimeout { get; }

        event EventHandler<UserActivityState>? StateChanged;
        event EventHandler? Activity;

        void NotifyActivity();
    }
}
