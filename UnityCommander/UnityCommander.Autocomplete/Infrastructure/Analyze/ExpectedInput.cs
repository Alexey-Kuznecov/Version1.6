
using UnityCommander.Abstractions.Completion;

namespace UnityCommander.Autocomplete.Infrastructure.Analyze
{
    //public sealed class ExpectedInput
    //{
    //    public ExpectedKind Kind { get; set; }
    //    public IReadOnlyList<string>? AllowedNames { get; set; }
    //    public ArgumentValueType ValueType { get; internal set; }

    //    public static ExpectedInput Command(IEnumerable<ICommandDescriptor> commands)
    //    {
    //        return new ExpectedInput
    //        {
    //            Kind = ExpectedKind.Command,
    //            AllowedNames = commands.Select(c => c.Name).ToList()
    //        };
    //    }

    //    public static ExpectedInput Variants(ICommandDescriptor command)
    //    {
    //        return new ExpectedInput
    //        {
    //            Kind = ExpectedKind.Command,
    //            AllowedNames = command.Variants.Select(v => v.Name).ToList()
    //        };
    //    }

    //    public static ExpectedInput Flags(ICommandDescriptor command)
    //    {
    //        return new ExpectedInput
    //        {
    //            Kind = ExpectedKind.Flag,
    //            AllowedNames = command.Variants.SelectMany(v => v.Flags).Select(f => f.Name).ToList()
    //        };
    //    }

    //    public static ExpectedInput FlagValue(ArgumentValueType? valueType)
    //    {
    //        return new ExpectedInput
    //        {
    //            Kind = ExpectedKind.FlagValue,
    //            ValueType = valueType,
    //        };
    //    }

    //    public static ExpectedInput Positionals(ICommandDescriptor command)
    //    {
    //        return new ExpectedInput
    //        {
    //            Kind = ExpectedKind.PositionalArgument,
    //            AllowedNames = command.Variants.SelectMany(v => v.Arguments).Select(a => a.Name).ToList()
    //        };
    //    }

    //    public static ExpectedInput Nothing()
    //    {
    //        return new ExpectedInput { Kind = ExpectedKind.Nothing };
    //    }
    //}
}
