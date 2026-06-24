
using UnityCommander.Abstractions.Dialog;

namespace UnityCommander.Modules.ToolBar.Commands
{
    public class ShowDialogCommand
    {
        private string _id;
        private IWindowManager _manager;

        public ShowDialogCommand(string id, IWindowManager manager)
        {
            _manager = manager;
            _id = id;
        }

        public void Execute()
        {
            _manager.ShowModalDialog(_id);
        }

        public bool CanExecute() => true;
    }
}
