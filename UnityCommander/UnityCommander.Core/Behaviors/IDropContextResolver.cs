
namespace UnityCommander.Core.Behaviors
{
    public interface IDropContextResolver
    {
        bool CanResolve(DragDropContext context);

        IDropContext Resolve(DragDropContext context);
    }
}
