
namespace UnityCommander.UI.Interaction
{
    public interface IContextAction
    {
        string Id { get; }

        string Name { get; }

        bool CanExecute(IInteractionContext context);

        Task ExecuteAsync(IInteractionContext context);
    }
}
