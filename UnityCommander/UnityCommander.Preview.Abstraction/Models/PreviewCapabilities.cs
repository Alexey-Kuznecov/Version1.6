
namespace UnityCommander.Preview.Abstraction.Models;

[Flags]
public enum PreviewCapabilities
{
    None = 0,

    Preview = 1,
    Edit = 2,
    Playback = 4,
    OpenExternally = 8
}

