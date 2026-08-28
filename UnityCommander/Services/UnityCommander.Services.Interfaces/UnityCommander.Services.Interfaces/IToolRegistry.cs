
namespace UnityCommander.Services.Interfaces
{
    public interface IToolRegistry
    {
        IToolDescriptor? Get(string id);
    }
}