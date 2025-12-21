
namespace UnityCommander.Common.Commands
{
    using System.Windows.Input;

    public interface IGlobalCommand : ICommandBase
    {
        public InputGesture ShortcutKey { get; set; }
        public GlobalCommandSelection SelectionFlag { get; set; }
    }
}
