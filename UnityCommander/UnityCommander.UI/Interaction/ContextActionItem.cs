
using System.Windows.Input;

namespace UnityCommander.UI.Interaction
{
    public sealed class ContextActionItem
    {
        public string Name { get; }
        public ICommand Command { get; }

        public ContextActionItem(
            IContextAction action,
            IInteractionContext context)
        {
            Name = action.Name;
            Command = new ContextActionCommand(action, context);
        }
    }
}
