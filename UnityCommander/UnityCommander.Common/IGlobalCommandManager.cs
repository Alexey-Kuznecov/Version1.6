using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using UnityCommander.Common.Models;
using UnityCommander.Integration.Enums;

namespace UnityCommander.Common
{
    public interface IGlobalCommandManager
    {
        ICommand GetCommand(string commandName);
    }
}
