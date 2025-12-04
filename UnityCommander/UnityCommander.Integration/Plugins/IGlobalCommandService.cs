
namespace UnityCommander.Services.Interfaces
{
    using Common;

    using UnityCommander.Common.Commands;

    public interface IGlobalCommandService
    {
        IGlobalCommandManager GetCommandManager();
    }
}
