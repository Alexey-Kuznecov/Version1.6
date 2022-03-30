using System;
using System.Collections.Generic;
using System.Text;

namespace UnityCommander.Core.Commands
{
    public class CommandMg
    {
        /// <summary>
        /// The navigation invoker.
        /// </summary>
        private readonly FileManagerInvoker fileManagerInvoker;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandManager"/> class.
        /// </summary>
        public CommandMg()
        {
            this.fileManagerInvoker = new FileManagerInvoker();
        }
    }
}
