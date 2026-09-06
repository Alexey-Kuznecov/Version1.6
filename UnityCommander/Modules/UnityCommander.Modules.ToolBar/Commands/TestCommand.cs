
using UnityCommander.Core.Plugin;
using UnityCommander.Ribbon.Core.Commands;

namespace UnityCommander.Modules.ToolBar.Commands
{
    public class TestCommand : RibbonCommand
    {
        private string _id;
        private IPluginCommandDispatcher _dispatcher;

        public TestCommand(string id, IPluginCommandDispatcher dispatcher) : base("test") 
        {
            _dispatcher = dispatcher;
            _id = id;
        }

        public override void Execute()
        {
            _dispatcher.ExecuteAsync(_id);
        }

        public override bool CanExecute() => true;
    }
}
