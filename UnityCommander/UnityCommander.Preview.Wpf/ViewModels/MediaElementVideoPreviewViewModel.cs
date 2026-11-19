
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using UnityCommander.Logging;
using UnityCommander.Mvvm.Base;
using UnityCommander.Preview.Abstraction.Interfaces;
using UnityCommander.Preview.Wpf.Models;

public sealed class MediaElementVideoPreviewViewModel
    : PropertiesChanged, IDisposable, IPreviewStateProvider
{
    private MediaElement? _mediaElement;
    private readonly DispatcherTimer _positionTimer;

    private long _position;
    private long _duration;
    private int _volume = 100;
    private bool _isMuted;
    private bool _isPlaying;
    private bool _isSeeking;
    private bool _isRepeatEnabled;
    private bool _isAutoPlayEnabled = true;
    private bool _isAtEnd;
    private bool _disposed;

    public MediaElementVideoPreviewViewModel()
    {
        PlayCommand = new DelegateCommand(Play);
        PauseCommand = new DelegateCommand(Pause);
        StopCommand = new DelegateCommand(Stop);

        ToggleMuteCommand = new DelegateCommand(
            () => IsMuted = !IsMuted);

        ToggleRepeatCommand = new DelegateCommand(
            () => IsRepeatEnabled = !IsRepeatEnabled);

        TogglePlayPauseCommand = new DelegateCommand(
            TogglePlayPause);

        _positionTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(200)
        };

        _positionTimer.Tick += OnPositionTimerTick;
    }

    public ICommand PlayCommand { get; }
    public ICommand PauseCommand { get; }
    public ICommand StopCommand { get; }
    public DelegateCommand ToggleMuteCommand { get; }
    public DelegateCommand ToggleRepeatCommand { get; }
    public DelegateCommand TogglePlayPauseCommand { get; }

    public MediaElement? MediaElement => _mediaElement;

    public long Position
    {
        get => _position;
        set
        {
            if (!SetProperty(ref _position, value))
                return;

            OnPropertyChanged(nameof(PositionText));

            if (_isSeeking && _mediaElement != null)
                _mediaElement.Position =
                    TimeSpan.FromMilliseconds(value);
        }
    }

    private long _seekPosition;

    public long SeekPosition
    {
        get => _seekPosition;
        set => SetProperty(ref _seekPosition, value);
    }

    public long Duration
    {
        get => _duration;
        private set
        {
            if (!SetProperty(ref _duration, value))
                return;

            OnPropertyChanged(nameof(DurationText));
        }
    }

    public int Volume
    {
        get => _volume;
        set
        {
            value = Math.Clamp(value, 0, 100);

            if (SetProperty(ref _volume, value) &&
                _mediaElement != null)
            {
                _mediaElement.Volume = value / 100.0;

                OnPropertyChanged(nameof(MediaVolume));
            }
        }
    }

    public bool IsMuted
    {
        get => _isMuted;
        set
        {
            if (SetProperty(ref _isMuted, value) &&
                _mediaElement != null)
            {
                _mediaElement.IsMuted = value;
            }
        }
    }

    public bool IsRepeatEnabled
    {
        get => _isRepeatEnabled;
        set => SetProperty(ref _isRepeatEnabled, value);
    }

    public bool IsAutoPlayEnabled
    {
        get => _isAutoPlayEnabled;
        set => SetProperty(ref _isAutoPlayEnabled, value);
    }

    public bool IsPlaying
    {
        get => _isPlaying;
        private set
        {
            if (SetProperty(ref _isPlaying, value))
                OnPropertyChanged(nameof(PlayPauseIcon));
        }
    }

    public string PlayPauseIcon => IsPlaying ? "Ⅱ" : "▶";
    public double MediaVolume => Volume / 100.0;
    public string PositionText => FormatTime(Position);
    public string DurationText => FormatTime(Duration);

    public string StateId => "preview.video.mediaelement";

    private static string FormatTime(long milliseconds)
    {
        var time = TimeSpan.FromMilliseconds(Math.Max(0, milliseconds));

        return time.TotalHours >= 1
            ? time.ToString(@"h\:mm\:ss")
            : time.ToString(@"m\:ss");
    }

    public void Attach(MediaElement mediaElement)
    {
        Detach();

        _mediaElement = mediaElement;

        _mediaElement.Volume = Volume / 100.0;
        _mediaElement.IsMuted = IsMuted;

        _mediaElement.MediaOpened += OnMediaOpened;
        _mediaElement.MediaEnded += OnMediaEnded;
        _mediaElement.MediaFailed += OnMediaFailed;

        OnPropertyChanged(nameof(MediaElement));
    }

    public void SetSource(Uri source)
    {
        if (_mediaElement == null)
            return;

        Stop();

        Duration = 0;
        Position = 0;
        _isAtEnd = false;

        _mediaElement.Source = source;
    }

    public void Play()
    {
        if (_mediaElement == null)
            return;

        if (_isAtEnd ||
            (Duration > 0 && Position >= Duration))
        {
            _isAtEnd = false;
            _mediaElement.Position = TimeSpan.Zero;
            Position = 0;
        }

        _mediaElement.Play();

        IsPlaying = true;
        _positionTimer.Start();
    }

    public void Pause()
    {
        if (_mediaElement == null)
            return;

        _mediaElement.Pause();

        IsPlaying = false;
        _positionTimer.Stop();
    }

    public void Stop()
    {
        if (_mediaElement == null)
            return;

        _positionTimer.Stop();
        _isAtEnd = false;

        _mediaElement.Stop();

        Position = 0;
        IsPlaying = false;
    }

    public void TogglePlayPause()
    {
        if (IsPlaying)
            Pause();
        else
            Play();
    }

    public void BeginSeek()
    {
        _isSeeking = true;
        SeekPosition = Position;
    }

    public void EndSeek()
    {
        if (_mediaElement == null)
            return;

        var target = SeekPosition;

        target = Math.Clamp(target, 0, Duration);

        _mediaElement.Position = TimeSpan.FromMilliseconds(target);

        Position = target;
        _isSeeking = false;
    }

    public void Seek(long position)
    {
        if (_mediaElement == null)
            return;

        position = Math.Clamp(position, 0, Duration);

        _mediaElement.Position = TimeSpan.FromMilliseconds(position);
        Position = position;
    }

    private void OnMediaOpened(object? sender, RoutedEventArgs e)
    {
        if (_mediaElement?.NaturalDuration.HasTimeSpan == true)
        {
            Duration = (long)_mediaElement
                .NaturalDuration.TimeSpan.TotalMilliseconds;
        }

        if (IsAutoPlayEnabled)
            Play();
    }

    private void OnMediaEnded(object? sender, RoutedEventArgs e)
    {
        _positionTimer.Stop();
        IsPlaying = false;
        _isAtEnd = true;

        if (IsRepeatEnabled)
        {
            _isAtEnd = false;
            _mediaElement!.Stop();
            _mediaElement.Position = TimeSpan.Zero;
            Position = 0;

            _mediaElement.Play();

            IsPlaying = true;
            _positionTimer.Start();
        }
        else
        {
            Position = Duration;
        }
    }

    private void OnMediaFailed(object? sender, ExceptionRoutedEventArgs e)
    {
        _positionTimer.Stop();
        IsPlaying = false;

        Log.Error(e.ErrorException,
            "[Video] MediaElement playback failed.");
    }

    private void OnPositionTimerTick(object? sender, EventArgs e)
    {
        if (_mediaElement == null || _isSeeking || _isAtEnd)
            return;

        Position = (long)_mediaElement.Position.TotalMilliseconds;
        SeekPosition = Position;
    }

    private void Detach()
    {
        if (_mediaElement == null)
            return;

        Stop();

        _mediaElement.MediaOpened -= OnMediaOpened;
        _mediaElement.MediaEnded -= OnMediaEnded;
        _mediaElement.MediaFailed -= OnMediaFailed;

        _mediaElement.Source = null;
        _mediaElement = null;

        OnPropertyChanged(nameof(MediaElement));
    }

    public void Clear()
    {
        Stop();

        if (_mediaElement != null)
            _mediaElement.Source = null;

        Duration = 0;
        Position = 0;
        IsPlaying = false;
        _isAtEnd = false;
    }

    public object CaptureState()
    {
        return new VideoPreviewState
        {
            Volume = Volume,
            IsMuted = IsMuted,
            IsRepeatEnabled = IsRepeatEnabled
        };
    }

    public void RestoreState(string state)
    {
        var videoState =
            JsonSerializer.Deserialize<VideoPreviewState>(state);

        if (videoState == null)
            return;

        Volume = videoState.Volume;
        IsMuted = videoState.IsMuted;
        IsRepeatEnabled = videoState.IsRepeatEnabled;
    }

    private void UnsubscribeTimer()
    {
        _positionTimer.Stop();
        _positionTimer.Tick -= OnPositionTimerTick;
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;

        UnsubscribeTimer();
        Detach();
    }
}
