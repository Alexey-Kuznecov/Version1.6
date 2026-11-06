
namespace UnityCommander.Notifications.Abstraction.Models
{
    public sealed class Notification
    {
        public Guid Id { get; init; } = Guid.NewGuid();

        public string Title { get; init; } = string.Empty;

        public string Message { get; init; } = string.Empty;

        public NotificationType Type { get; init; }

        public NotificationOptions Options { get; init; } = new();
    }
}
