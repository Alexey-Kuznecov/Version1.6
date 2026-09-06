
using System.Threading.Tasks;
using UnityCommander.Abstractions.Overrides;

namespace UnityCommander.Common.Override.Engine
{
    public class FileOperationService : IFileOperationService
    {
        private readonly IFileCopyEngine _engine;

        public FileOperationService(IFileCopyEngine engine)
        {
            _engine = engine;
        }

        public Task CopyAsync(FileOperationRequest request)
        {
            return _engine.StartAsync(request);
        }
    }
}
