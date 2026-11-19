
using LibVLCSharp.Shared;
using System.Diagnostics;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using UnityCommander.Logging;
using UnityCommander.Mvvm.Base;
using UnityCommander.Preview.Abstraction.Interfaces;
using UnityCommander.Preview.Wpf.Models;

namespace UnityCommander.Preview.Wpf.ViewModels;

public sealed class VideoPreviewViewModel : PropertiesChanged, IDisposable, IPreviewStateProvider
{
    private MediaPlayer? _mediaPlayer;

    private long _position;
    private long _duration;
    private int _volume = 100;
    private bool _isMuted;
    private bool _isPlaying;
    private bool _isSeeking;

    private bool _isAtEnd;

    public MediaPlayer? MediaPlayer => _mediaPlayer;

    public VideoPreviewViewModel()
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
    }

    public ICommand PlayCommand { get; }
    public ICommand PauseCommand { get; }
    public ICommand StopCommand { get; }
    public ICommand ToggleMuteCommand { get; }
    public ICommand ToggleRepeatCommand { get; }

    public long Position
    {
        get => _position;
        set
        {
            if (!SetProperty(ref _position, value))
                return;

            Log.Debug($"[Video] Position SET: {value}");
            //Log.Debug(Environment.StackTrace);

            if (_isSeeking && _mediaPlayer != null)
                _mediaPlayer.Time = value;
        }
    }

    public long Duration
    {
        get => _duration;
        private set => SetProperty(ref _duration, value);
    }

    public int Volume
    {
        get => _volume;
        set
        {
            value = Math.Clamp(value, 0, 100);

            if (SetProperty(ref _volume, value) &&
                _mediaPlayer != null)
            {
                _mediaPlayer.Volume = value;
            }
        }
    }

    public bool IsMuted
    {
        get => _isMuted;
        set
        {
            if (SetProperty(ref _isMuted, value) &&
                _mediaPlayer != null)
            {
                _mediaPlayer.Mute = value;
            }
        }
    }

    private bool _isRepeatEnabled;

    public bool IsRepeatEnabled
    {
        get => _isRepeatEnabled;
        set => SetProperty(ref _isRepeatEnabled, value);
    }

    public bool IsPlaying
    {
        get => _isPlaying;
        set
        {
            if (SetProperty(ref _isPlaying, value))
            {
                OnPropertyChanged(nameof(PlayPauseIcon));
            }
        }
    }

    public DelegateCommand TogglePlayPauseCommand { get; }

    public string PlayPauseIcon => IsPlaying ? "Ⅱ" : "▶";

    public string PositionText => FormatTime(Position);

    public string DurationText => FormatTime(Duration);

    private static string FormatTime(long milliseconds)
    {
        var time = TimeSpan.FromMilliseconds(Math.Max(0, milliseconds));

        return time.TotalHours >= 1
            ? time.ToString(@"h\:mm\:ss")
            : time.ToString(@"m\:ss");
    }

    public void SetMediaPlayer(MediaPlayer mediaPlayer)
    {
        if (_mediaPlayer != null)
            Unsubscribe();

        _mediaPlayer = mediaPlayer;

        _mediaPlayer.Volume = Volume;

        _mediaPlayer.Playing += OnPlaying;
        _mediaPlayer.Paused += OnPaused;
        _mediaPlayer.Stopped += OnStopped;
        _mediaPlayer.LengthChanged += OnLengthChanged;
        _mediaPlayer.TimeChanged += OnTimeChanged;
        _mediaPlayer.EndReached += OnEndReached;

        OnPropertyChanged(nameof(MediaPlayer));
    }

    public void Play()
    {
        if (_mediaPlayer == null)
            return;

        if (_isAtEnd || (Duration > 0 && Position >= Duration))
        {
            _isAtEnd = false;

            _mediaPlayer.Stop();
            _mediaPlayer.Time = 0;

            Position = 0;
            OnPropertyChanged(nameof(PositionText));
        }

        _mediaPlayer.Play();
    }

    public void Stop()
    {
        if (_mediaPlayer == null)
            return;

        _isAtEnd = false;

        _mediaPlayer.Stop();

        Position = 0;
        OnPropertyChanged(nameof(PositionText));
    }

    public void Pause()
    {
        _mediaPlayer?.Pause();
    }

    private void OnTimeChanged(object? sender, MediaPlayerTimeChangedEventArgs e)
    {
        Log.Debug(
        $"[Video] TimeChanged: {e.Time}, IsAtEnd: {_isAtEnd}, IsSeeking: {_isSeeking}");

        if (!_isSeeking && !_isAtEnd)
        {
            Position = e.Time;
            OnPropertyChanged(nameof(PositionText));
        }
    }

    private void OnEndReached(object? sender, EventArgs e)
    {
        Log.Debug("[Video] EndReached");

        _isAtEnd = true;

        Application.Current.Dispatcher.BeginInvoke(() =>
        {
            if (_mediaPlayer == null)
                return;

            if (IsRepeatEnabled)
            {
                _isAtEnd = false;

                _mediaPlayer.Time = 0;
                Position = 0;

                _mediaPlayer.Play();
            }
            else
            {
                Position = Duration;
                IsPlaying = false;
                OnPropertyChanged(nameof(PositionText));
            }
        });

        Debug.WriteLine(
            $"[Video] EndReached completed: Position={Position}, Duration={Duration}");
    }

    public void TogglePlayPause()
    {
        if (_mediaPlayer == null)
            return;

        if (IsPlaying)
            Pause();
        else
            Play();
    }

    public void Seek(long position)
    {
        if (_mediaPlayer == null)
            return;

        _mediaPlayer.Time = Math.Clamp(position, 0, Duration);
    }

    public void BeginSeek()
    {
        _isSeeking = true;
    }

    public void EndSeek()
    {
        _isSeeking = false;
        Seek(Position);
    }

    private void OnPlaying(object? sender, EventArgs e)
    {
        IsPlaying = true;
    }

    private void OnPaused(object? sender, EventArgs e)
    {
        IsPlaying = false;
    }

    private void OnStopped(object? sender, EventArgs e)
    {
        IsPlaying = false;
    }

    private void OnLengthChanged(object? sender, MediaPlayerLengthChangedEventArgs e)
    {
        Duration = e.Length;
        OnPropertyChanged(nameof(DurationText));
    }

    private void Unsubscribe()
    {
        _mediaPlayer!.Playing -= OnPlaying;
        _mediaPlayer.Paused -= OnPaused;
        _mediaPlayer.Stopped -= OnStopped;
        _mediaPlayer.LengthChanged -= OnLengthChanged;
        _mediaPlayer.TimeChanged -= OnTimeChanged;
        _mediaPlayer.EndReached -= OnEndReached;
    }

    public void Clear()
    {
        Stop();

        Duration = 0;
        Position = 0;
        IsPlaying = false;
    }

    public void Dispose()
    {
        if (_mediaPlayer == null)
            return;

        Unsubscribe();
        _mediaPlayer.Dispose();
        _mediaPlayer = null;

        OnPropertyChanged(nameof(MediaPlayer));
    }

    public string StateId => "preview.video";

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
        var videoState = JsonSerializer.Deserialize<VideoPreviewState>(state);

        if (videoState == null)
            return;

        Volume = videoState.Volume;
        IsMuted = videoState.IsMuted;
        IsRepeatEnabled = videoState.IsRepeatEnabled;
    }
}
