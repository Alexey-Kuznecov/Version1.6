using System.Threading.Tasks;

namespace UnityCommander.Abstractions.Overrides
{
    public interface IFileCopyEngine
    {
        Task StartAsync(FileOperationRequest request);
    }
}