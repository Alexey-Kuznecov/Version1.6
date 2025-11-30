using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommander.CLI.Core
{
    public interface ICommandParameters
    {
        void Set(string key, object? value);
        T? Get<T>(string key);
        bool Contains(string key);
    }
}
