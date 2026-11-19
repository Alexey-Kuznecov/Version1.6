using LibVLCSharp.Shared;

namespace UnityCommander.Preview.Wpf;

public sealed class VideoEngine : IDisposable
{
    public LibVLC LibVLC { get; }

    public VideoEngine()
    {
        LibVLC = new LibVLC();
    }

    public void Dispose()
    {
        LibVLC.Dispose();
    }
}