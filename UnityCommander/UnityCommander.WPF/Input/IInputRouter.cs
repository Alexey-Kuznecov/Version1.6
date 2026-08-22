
using System.Windows;
using System.Windows.Input;

namespace UnityCommander.WPF.Input
{
    public interface IInputRouter
    {
        void Process(Window source, KeyEventArgs e);
    }
}
