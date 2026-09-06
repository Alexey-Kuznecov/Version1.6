
namespace UnityCommander.Services.Interfaces
{
    public interface IToolRegistry
    {
        IToolDescriptor FindByContentId(string contentId);

        IToolDescriptor? Get(string id);
    }
}