
using System.Windows.Threading;

namespace UnityCommander.Notifications.WPF.Infrastructure;

public sealed class NotificationTimer : IDisposable
{
    private readonly DispatcherTimer _timer;
    private readonly Action _onElapsed;

    public NotificationTimer(
        TimeSpan duration,
        Action onElapsed)
    {
        _onElapsed = onElapsed;

        _timer = new DispatcherTimer
        {
            Interval = duration
        };

        _timer.Tick += OnTick;
    }

    public void Start()
    {
        _timer.Start();
    }

    public void Pause()
    {
        _timer.Stop();
    }

    public void Resume()
    {
        _timer.Start();
    }

    private void OnTick(object? sender, EventArgs e)
    {
        Dispose();
        _onElapsed();
    }

    public void Dispose()
    {
        _timer.Stop();
        _timer.Tick -= OnTick;
    }
}