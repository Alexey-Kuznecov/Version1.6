using System;
using System.Collections.Generic;
using System.Text;

namespace UnityCommander.Core.Commands
{
    public interface IParamProvider
    {
        void GetParams(object parameters);
    }
}
