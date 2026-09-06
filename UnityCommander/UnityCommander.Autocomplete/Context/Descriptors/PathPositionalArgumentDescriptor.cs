
using UnityCommander.Abstractions.Completion;

namespace UnityCommander.Autocomplete.Context.Descriptors
{
    public sealed class PathPositionalArgumentDescriptor
     : IPositionalArgumentDescriptor,
       IPathValueDescriptor
    {
        public string Name { get; }
        
        public ArgumentValueType ValueType => ArgumentValueType.Path;

        public PathKind PathKind { get; }

        public bool IsRequired { get; }
        public bool IsRepeatable { get; }

        public PathPositionalArgumentDescriptor(
            string name,
            PathKind pathKind = PathKind.Any,
            bool isRequired = false,
            bool isRepeatable = false)
        {
            Name = name;
            PathKind = pathKind;
            IsRequired = isRequired;
            IsRepeatable = isRepeatable;
        }
    }
}
