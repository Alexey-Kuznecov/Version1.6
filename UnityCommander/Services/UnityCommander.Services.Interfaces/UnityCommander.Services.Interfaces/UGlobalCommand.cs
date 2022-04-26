using System.Collections.Generic;
using System.Windows.Input;
using UnityCommander.Common;

namespace UnityCommander.Services.Interfaces
{
    public class UGlobalCommand
    {
        public object ControlItem { get; set; }

        public XParamViewModel XParamViewModel { get; set; }
        
        public List<XParamViewModel> XParamModelList { get; set; }

        public ICommand Command { get; set; }

        public InputGesture ShortcutKey { get; set; }
    }
}
