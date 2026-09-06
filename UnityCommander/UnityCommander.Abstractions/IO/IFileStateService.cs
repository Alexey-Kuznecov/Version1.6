
namespace UnityCommander.Common.Models
{
    public interface IFileStateService
    {
        public IFileState GetState(string path);

        //bool TryGet(string path, out IFileState state);

        void Set(Guid operationId, IFileState state);

        void Remove(Guid operationId);
    }
}
