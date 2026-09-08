
namespace UnityCommander.UI.Interaction
{
    public interface IUserInteraction
    {
        string Type { get; }

        IInteractionContext Context { get; }

        DateTime Timestamp { get; }
    }
}
