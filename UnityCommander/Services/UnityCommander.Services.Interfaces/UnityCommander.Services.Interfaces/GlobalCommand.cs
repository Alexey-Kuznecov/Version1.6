using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using UnityCommander.Common;

namespace UnityCommander.Services
{
    public class GlobalCommand
    {
        public object ControlItem { get; set; }

        public XParamModel XParamModel { get; set; }
        
        public List<XParamModel> XParamModelList { get; set; }

        public ICommand Command { get; set; }

        public InputGesture ShortcutKey { get; set; }
    }
}
