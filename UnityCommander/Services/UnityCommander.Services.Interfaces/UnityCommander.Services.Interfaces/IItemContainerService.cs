
using System.Windows;

namespace UnityCommander.Services.Interfaces
{
    public interface IItemContainerService
    {
        FrameworkElement? GetContainer(object item);
    }
}
