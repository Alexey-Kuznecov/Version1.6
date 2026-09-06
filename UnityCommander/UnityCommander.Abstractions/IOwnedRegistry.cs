
namespace UnityCommander.Abstractions
{
    public interface IOwnedRegistry
    {
        void Cleanup(string ownerId);
    }
}
