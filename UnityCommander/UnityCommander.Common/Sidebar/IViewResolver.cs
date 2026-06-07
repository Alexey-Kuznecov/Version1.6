using System.Windows.Controls;

namespace UnityCommander.Common.Sidebar
{
    public interface IViewResolver
    {
        UserControl Resolve(string viewKey);
    }
}