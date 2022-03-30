using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using UnityCommander.Common.Models;

namespace UnityCommander.Services.Interfaces
{
    public interface IGlobalCommandService
    {
        public void SetCommand<T>();
        UCCommand GetCommand<T>(string commandName);
    }
}
