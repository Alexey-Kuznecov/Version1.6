using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using UnityCommander.Common;
using UnityCommander.Common.Models;

namespace UnityCommander.Services.Interfaces
{
    public delegate void SetParamDelegate(object view, object propertyName, object control, string commandName);

    public interface IGlobalCommandService
    {
        void SetCommand<T>(string commandName, UGlobalCommand uGlobal);

        void SetCommand(UGlobalCommand uGlobal);

        GlobalCommand GetCommand<T>(string commandName);
        
        GlobalCommand GetCommand(string commandName);

        IGlobalCommandManager GetCommandManager<T>();
    }
}
