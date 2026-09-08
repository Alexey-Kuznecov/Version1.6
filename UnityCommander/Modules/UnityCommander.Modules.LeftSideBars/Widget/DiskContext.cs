
using UnityCommander.UI.Interaction;

namespace UnityCommander.Modules.LeftSideBars.Widget
{
    public sealed record DiskContext(
     string Path) : IInteractionContext;
}
