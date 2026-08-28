
using System.Collections.Generic;

namespace UnityCommander.Modules.BottomPanel.Services
{
    public sealed record StartupCommand(
      string CommandName,
      IReadOnlyDictionary<string, object?> Arguments);
}
