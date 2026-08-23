
namespace UnityCommander.Abstractions.Background
{
    public interface IBackgroundResourcePolicy
    {
        BackgroundPriority Priority { get; }

        event EventHandler<BackgroundPriority>? PriorityChanged;
    }
}
