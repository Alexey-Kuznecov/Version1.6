
using System;

namespace UnityCommander.Common.Docking
{
    public class MoveTab
    {
        public Guid TabId { get; set; }
        public Guid FromPanel { get; set; }
        public Guid ToPanel { get; set; }
    }
}
