
namespace UnityCommander.Preview.Abstraction.Models;

public abstract class PreviewContent : IDisposable
{
    public PreviewKind Kind { get; }

    protected PreviewContent(PreviewKind kind)
    {
        Kind = kind;
    }

    public virtual void Dispose()
    {
    }
}

