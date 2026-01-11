using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommander.Autocomplete.Infrastructure.Analyze
{
    public readonly struct ReplaceRange
    {
        public int Start { get; }
        public int Length { get; }
    }
}
