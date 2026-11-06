
using Prism.Ioc;
using UnityCommander.Notifications.Abstraction.Contracts;
using UnityCommander.Notifications.WPF.Presentation.ViewModels;
using UnityCommander.Notifications.WPF.Services;

namespace UnityCommander.Dependencies
{
    public class NotificationRegistration
    {
        public static void Register(IContainerRegistry registry)
        {
            registry.RegisterSingleton<NotificationCollection>();
            registry.RegisterSingleton<NotificationHostViewModel>();
            registry.RegisterSingleton<INotificationService, NotificationService>();
        }
    }
}
