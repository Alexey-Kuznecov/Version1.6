
using UnityCommander.Common.Dialog;
using UnityCommander.Ribbon.Core.Commands;

namespace UnityCommander.Modules.ToolBar.Commands
{
    public class ShowDialogCommand : RibbonCommand
    {
        private string _id;
        private IWindowManager _manager;

        public ShowDialogCommand(string id, IWindowManager manager) : base("test") 
        {
            _manager = manager;
            _id = id;
        }

        public override void Execute()
        {
            _manager.ShowModalDialog(_id);
        }

        public override bool CanExecute() => true;
    }
}
