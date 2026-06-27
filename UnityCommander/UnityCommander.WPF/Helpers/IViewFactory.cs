
using System.Windows.Controls;

namespace UnityCommander.WPF.Helper
{
    public interface IViewFactory
    {
        public UserControl Create(Type viewType);
    }
}