
using System.Windows.Media;
using UnityCommander.Mvvm.Base;
using UnityCommander.Notifications.Abstraction.Models;

namespace UnityCommander.Notifications.WPF.Presentation.ViewModels;

public sealed class NotificationViewModel : PropertiesChanged
{
    private static readonly Brush InfoBrush = CreateBrush(75, 155, 232);
    private static readonly Brush SuccessBrush = CreateBrush(64, 183, 122);
    private static readonly Brush WarningBrush = CreateBrush(229, 184, 75);
    private static readonly Brush ErrorBrush = CreateBrush(224, 93, 93);

    public Guid Id { get; }

    public string? Title { get; }

    public string Message { get; }

    public NotificationType Type { get; }

    public bool IsDismissible { get; }

    public string IconKey { get; }

    public Brush AccentBrush { get; }

    public RelayCommand CloseCommand { get; }

    public NotificationViewModel(
        Notification notification,
        Action<Guid> onClose)
    {
        Id = notification.Id;
        Title = notification.Title;
        Message = notification.Message;
        Type = notification.Type;
        IsDismissible = notification.Options.IsDismissible;

        (IconKey, AccentBrush) = GetAppearance(Type);

        CloseCommand = new RelayCommand(
            () => onClose(Id));
    }

    private static (string IconKey, Brush AccentBrush) GetAppearance(
        NotificationType type)
    {
        return type switch
        {
            NotificationType.Info =>
                ("Info", InfoBrush),

            NotificationType.Success =>
                ("Check", SuccessBrush),

            NotificationType.Warning =>
                ("AlertTriangle", WarningBrush),

            NotificationType.Error =>
                ("AlertCircle", ErrorBrush),

            _ => ("Info", InfoBrush)
        };
    }

    private static Brush CreateBrush(
        byte red,
        byte green,
        byte blue)
    {
        var brush = new SolidColorBrush(
            Color.FromRgb(red, green, blue));

        brush.Freeze();

        return brush;
    }
}
