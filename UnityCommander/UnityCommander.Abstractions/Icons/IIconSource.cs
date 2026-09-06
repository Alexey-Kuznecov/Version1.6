
namespace UnityCommander.Abstractions.Icons
{
    public interface IIconSource
    {
        int Priority { get; }

        bool TryGet(
            string key,
            out RuntimeIcon icon);
    }
}
