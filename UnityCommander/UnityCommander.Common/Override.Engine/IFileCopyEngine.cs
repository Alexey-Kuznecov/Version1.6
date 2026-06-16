using System.Threading.Tasks;

namespace UnityCommander.Common.Override.Engine
{
    public interface IFileCopyEngine
    {
        Task StartAsync(FileOperationRequest request);
    }
}