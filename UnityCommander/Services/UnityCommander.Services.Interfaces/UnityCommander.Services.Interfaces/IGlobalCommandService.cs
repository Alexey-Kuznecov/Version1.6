using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using UnityCommander.Common.Models;

namespace UnityCommander.Services.Interfaces
{
    public delegate void SetParamDelegate(object view, object propertyName, object control, string commandName);

    public interface IGlobalCommandService
    {
        void SetCommand<T>();

        void SetCommand<T>(string commandName, GlobalCommand global);

        void SetCommand(GlobalCommand global);

        UCCommand GetCommand<T>(string commandName);
    }
}
