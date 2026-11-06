
namespace UnityCommander.Notifications.Abstraction.Models
{
    public sealed class NotificationOptions
    {
        public TimeSpan? Duration { get; set; } = TimeSpan.FromSeconds(5);

        public bool IsDismissible { get; set; } = true;

        public bool IsPersistent { get; set; }
    }
}
