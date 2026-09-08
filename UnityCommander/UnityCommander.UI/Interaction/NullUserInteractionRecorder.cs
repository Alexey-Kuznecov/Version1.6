
namespace UnityCommander.UI.Interaction
{
    public sealed class NullUserInteractionRecorder
        : IUserInteractionRecorder
    {
        public void Record(IUserInteraction interaction)
        {
        }
    }
}
