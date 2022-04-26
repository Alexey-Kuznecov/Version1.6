
namespace UnityCommander.Services.Interfaces
{
    using Common;

    public interface IGlobalCommandService
    {
        void SetCommand(UGlobalCommand uGlobal);

        IGlobalCommandManager GetCommandManager<T>();
    }
}
