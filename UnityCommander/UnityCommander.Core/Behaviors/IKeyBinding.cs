using System;
using System.Collections.Generic;
using System.Text;

namespace UnityCommander.Core.Behaviors
{
    public interface IKeyBinding
    {
        void SetBinding(object dependencyObject, KeyboardManager manager);
    } 
}
