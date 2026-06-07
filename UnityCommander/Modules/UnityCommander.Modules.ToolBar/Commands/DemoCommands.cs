
using System.Windows;
using UnityCommander.Ribbon.Core.Commands;

namespace UnityCommander.Modules.ToolBar.Commands
{
    public class DemoCommands : RibbonCommand
    {
        public DemoCommands() : base("test") { }

        public override void Execute() => MessageBox.Show("Работает!");

        public override bool CanExecute() => true;
    }
}
