
using System.Threading.Tasks;

namespace UnityCommander.Core.Behaviors
{
    public interface IDragDropHandler
    {
        bool CanHandle(IDropContext context);

        DragDropResult DragOver(
            IDropContext dropContext,
            DragDropContext context);

        Task DropAsync(
            IDropContext dropContext,
            DragDropContext context);
    }
}
