
using System.Windows;
using System.Windows.Input;

namespace UnityCommander.WPF.Behaviors
{
    public interface IInputRouter
    {
        void Process(Window source, KeyEventArgs e);
    }
}
