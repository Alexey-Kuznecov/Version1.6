using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommander.CLI.Core
{
    public interface IConsoleInput
    {
        Task<string?> ReadLineAsync(CancellationToken token);
        void Submit(string text);
    }
}
