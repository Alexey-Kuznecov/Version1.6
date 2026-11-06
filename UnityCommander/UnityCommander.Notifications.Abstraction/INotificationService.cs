
using UnityCommander.Notifications.Abstraction.Models;

namespace UnityCommander.Notifications.Abstraction.Contracts;

public interface INotificationService
{
    void Show(
        string message,
        NotificationType type,
        string? title = null,
        NotificationOptions? options = null);

    void ShowInfo(string message, string? title = null);

    void ShowSuccess(string message, string? title = null);

    void ShowWarning(string message, string? title = null);

    void ShowError(string message, string? title = null);
}
