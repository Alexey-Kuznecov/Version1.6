
using System.Windows.Input;

namespace UnityCommander.WPF.Behaviors
{
    public interface IInputService
    {
        public void Process(KeyEventArgs e);
    }
}