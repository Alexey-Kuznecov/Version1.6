
namespace UnityCommander.UI.Interaction
{
    public sealed record SelectionInteraction(
     IInteractionContext Context,
     DateTime Timestamp) : IUserInteraction
    {
        public string Type => "selection";
    }
}
