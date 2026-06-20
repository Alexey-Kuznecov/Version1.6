
namespace UnityCommander.Abstractions.Resources
{
    public interface IIconSource
    {
        bool TryGet(
            string key,
            out IconDefinition icon);
    }
}
