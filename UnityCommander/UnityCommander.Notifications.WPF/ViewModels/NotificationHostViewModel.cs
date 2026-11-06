using System.Collections.ObjectModel;
using System.Collections.Specialized;
using UnityCommander.Mvvm.Base;
using UnityCommander.Notifications.WPF.Services;

namespace UnityCommander.Notifications.WPF.Presentation.ViewModels;

public sealed class NotificationHostViewModel : PropertiesChanged
{
    private readonly NotificationCollection _collection;

    public ReadOnlyObservableCollection<NotificationViewModel> Notifications =>
        _collection.Items;

    public bool HasNotifications => Notifications.Count > 0;

    public NotificationHostViewModel(NotificationCollection collection)
    {
        _collection = collection;

        ((INotifyCollectionChanged)Notifications).CollectionChanged +=
            OnNotificationsChanged;
    }

    private void OnNotificationsChanged(
        object? sender,
        NotifyCollectionChangedEventArgs e)
    {
        OnPropertyChanged(nameof(HasNotifications));
    }
}