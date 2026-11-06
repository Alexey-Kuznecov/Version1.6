using System.Collections.ObjectModel;
using UnityCommander.Notifications.WPF.Presentation.ViewModels;

namespace UnityCommander.Notifications.WPF.Services;

public sealed class NotificationCollection
{
    private readonly ObservableCollection<NotificationViewModel> _items = new();

    public ReadOnlyObservableCollection<NotificationViewModel> Items { get; }

    public NotificationCollection()
    {
        Items = new ReadOnlyObservableCollection<NotificationViewModel>(
            _items);
    }

    public void Add(NotificationViewModel notification)
    {
        _items.Add(notification);
    }

    public bool Remove(Guid id)
    {
        var notification = _items.FirstOrDefault(
            item => item.Id == id);

        if (notification == null)
            return false;

        return _items.Remove(notification);
    }
}