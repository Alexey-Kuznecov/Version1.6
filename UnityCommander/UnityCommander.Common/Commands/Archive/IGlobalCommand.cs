
namespace UnityCommander.Common.Commands
{
    using System;
    using System.Windows.Input;

    [Obsolete]
    public interface IGlobalCommand : ICommandBase
    {
        public InputGesture ShortcutKey { get; set; }
        public GlobalCommandSelection SelectionFlag { get; set; }
    }
}
