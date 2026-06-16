
using System.Threading.Tasks;

namespace UnityCommander.Common.Override.Engine
{
    public interface IFileOperationService
    {
        Task CopyAsync(FileOperationRequest request);
    }
}
