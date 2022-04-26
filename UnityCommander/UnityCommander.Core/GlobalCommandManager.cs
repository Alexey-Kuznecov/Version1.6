using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using UnityCommander.Common;
using UnityCommander.Common.Models;
using UnityCommander.Integration.Commands;
using UnityCommander.Integration.Enums;

namespace UnityCommander.Core
{
    public class GlobalCommandManager : IGlobalCommandManager
    {
        private readonly List<GlobalCommand> globalCommands;

        public GlobalCommandManager(List<GlobalCommand> commands)
        {
            this.globalCommands = commands;
        }

        public ICommand GetCommand(string commandName) 
            => this.globalCommands.Single(c => c.Name == commandName).Command;
        
    }
}
