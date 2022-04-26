using System;
using System.Collections.Generic;
using System.Text;

namespace UnityCommander.Integration.Commands
{
    public interface ICommandFactory
    {
        void CommandFactory(CommandBuilder command);
    }
}
