using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommander.Common.Selection
{
    public class SelectionAction
    {
        public SelectionActionType Type { get; set; }
        public int TargetIndex { get; set; }
        public string Parameter { get; set; }
    }
}
