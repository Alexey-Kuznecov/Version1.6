using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommander.Plugins
{
    internal interface IPluginContext
    {
        T? GetService<T>() where T : class;
    }
}
