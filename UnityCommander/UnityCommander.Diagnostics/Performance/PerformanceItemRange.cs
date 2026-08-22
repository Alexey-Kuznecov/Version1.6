
namespace UnityCommander.Diagnostics.Performance
{
    public sealed record PerformanceItemRange(
     long Min,
     long? Max)
    {
        public bool Contains(long value)
        {
            if (value < Min)
                return false;

            return Max is null || value <= Max;
        }

        public override string ToString()
        {
            return Max is null
                ? $"{Min}+"
                : $"{Min}-{Max}";
        }
    }
}
