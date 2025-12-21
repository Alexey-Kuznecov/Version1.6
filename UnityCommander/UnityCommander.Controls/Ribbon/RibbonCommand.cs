
namespace UnityCommander.Controls.Ribbon
{
    using System.Windows.Input;
    using Common.Commands;

    public class RibbonCommand : IGlobalCommand
    {
        public string Name { get; set; }
        public ICommand Command { get; set; }
        public object CommandParameter { get; set; }
        public InputGesture ShortcutKey { get; set; }
        public GlobalCommandSelection SelectionFlag { get; set; }
    }
}
