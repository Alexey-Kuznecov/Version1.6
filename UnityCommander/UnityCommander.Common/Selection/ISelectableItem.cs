using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommander.Common.Selection
{
    public interface ISelectableItem
    {
        bool IsSelected { get; set; }
        string Key { get; }
    }
}
