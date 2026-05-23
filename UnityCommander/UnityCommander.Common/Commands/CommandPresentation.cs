using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommander.Common.Commands
{
    public record CommandPresentation(
        string DisplayName,
        string Description
    )
    {
        public static CommandPresentation Fallback(string id)
            => new(id, null);
    }
}
