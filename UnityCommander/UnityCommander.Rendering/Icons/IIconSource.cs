
namespace UnityCommander.Rendering.Icons
{
    public interface IIconSource
    {
        int Priority { get; }

        bool TryGet(
            string key,
            out RuntimeIcon icon);
    }
}
