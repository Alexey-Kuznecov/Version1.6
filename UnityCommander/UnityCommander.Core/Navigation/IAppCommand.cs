
namespace UnityCommander.Core.Navigation
{
    public interface IAppCommand
    {
        void Execute();
        void Undo();
    }
}
