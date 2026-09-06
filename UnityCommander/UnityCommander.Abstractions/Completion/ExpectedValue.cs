
namespace UnityCommander.Abstractions.Completion
{
    public sealed record ExpectedValue(
         IValueDescriptor Descriptor,
         CompletionKind Kind,
         ArgumentValueType ValueType);
}
