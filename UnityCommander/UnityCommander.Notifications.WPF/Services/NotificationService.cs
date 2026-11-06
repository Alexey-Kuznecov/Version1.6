using System.Windows;
using System.Windows.Threading;
using UnityCommander.Notifications.Abstraction.Contracts;
using UnityCommander.Notifications.Abstraction.Models;
using UnityCommander.Notifications.WPF.Infrastructure;
using UnityCommander.Notifications.WPF.Presentation.ViewModels;

namespace UnityCommander.Notifications.WPF.Services;

public sealed class NotificationService : INotificationService
{
    private readonly NotificationCollection _collection;

    private readonly Dictionary<Guid, NotificationTimer> _timers = new();

    public NotificationService(
        NotificationCollection collection)
    {
        _collection = collection;
    }

    public void Show(
      string message,
      NotificationType type,
      string? title = null,
      NotificationOptions? options = null)
    {
        options ??= new NotificationOptions();

        var notification = new Notification
        {
            Message = message,
            Title = title,
            Type = type,
            Options = options
        };

        Application.Current.Dispatcher.InvokeAsync(() =>
        {
            var viewModel = new NotificationViewModel(
                notification,
                Dismiss);

            _collection.Add(viewModel);

            if (!options.Duration.HasValue)
                return;

            var duration = options.Duration.Value;

            if (duration <= TimeSpan.Zero)
            {
                Dismiss(notification.Id);
                return;
            }

            var timer = new NotificationTimer(
                duration,
                () => Dismiss(notification.Id));

            _timers.Add(notification.Id, timer);
            timer.Start();
        });
    }

    public void ShowInfo(string message, string? title = null)
    {
        Show(message, NotificationType.Info, title);
    }

    public void ShowSuccess(string message, string? title = null)
    {
        Show(message, NotificationType.Success, title);
    }

    public void ShowWarning(string message, string? title = null)
    {
        Show(message, NotificationType.Warning, title);
    }

    public void ShowError(string message, string? title = null)
    {
        Show(message, NotificationType.Error, title);
    }

    private void Dismiss(Guid id)
    {
        Application.Current.Dispatcher.InvokeAsync(() =>
        {
            if (_timers.Remove(id, out var timer))
                timer.Dispose();

            _collection.Remove(id);
        });
    }
}