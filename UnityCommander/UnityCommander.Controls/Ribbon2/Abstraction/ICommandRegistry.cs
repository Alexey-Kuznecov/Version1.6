
using System.Windows.Input;

namespace UnityCommander.Controls.Ribbon2.Abstraction
{
    public interface ICommandRegistry
    {
        void Register(string id, ICommand command);
        ICommand Resolve(string id);
    }
}
