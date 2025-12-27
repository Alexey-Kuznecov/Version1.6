using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommander.Autocomplete.Context.Descriptors
{
    public class CommandVariant
    {
        public string Name; // mode-значение, например "commit"
        public List<IPositionalArgumentDescriptor> Arguments; // аргументы для этого режима
        public List<SimpleFlagDescriptor> Flags; // флаги для этого режима
    }
}
