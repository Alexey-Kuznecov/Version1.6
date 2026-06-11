
using UnityCommander.Common.Dialog;
using UnityCommander.Ribbon.Core.Commands;

namespace UnityCommander.Modules.ToolBar.Commands
{
    public class ShowDialogCommand : RibbonCommand
    {
        private IWindowManager _manager;

        public ShowDialogCommand(IWindowManager manager) : base("test") 
        {
            _manager = manager;
        }

        public override void Execute()
        {
            _manager.ShowModalDialog("icon_maker-1.0");
        }

        public override bool CanExecute() => true;
    }
}
