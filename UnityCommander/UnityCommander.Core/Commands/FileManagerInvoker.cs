using System;
using System.Collections.Generic;
using System.Text;
using UnityCommander.Core.Commands.Base;

namespace UnityCommander.Core.Commands
{
    public class FileManagerInvoker : InvokerBase
    {
        public override void Execute(Action action)
        {
            throw new NotImplementedException();
        }

        public override void Execute(Action<object> action, object path)
        {
            throw new NotImplementedException();
        }

        public override void UnExecute(Action action)
        {
            throw new NotImplementedException();
        }

        public override void UnExecute(Action<object> action, object arg)
        {
            throw new NotImplementedException();
        }
    }
}
