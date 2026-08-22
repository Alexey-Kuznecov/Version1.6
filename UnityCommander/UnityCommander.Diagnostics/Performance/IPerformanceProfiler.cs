
namespace UnityCommander.Diagnostics.Performance
{
    public interface IPerformanceProfiler
    {
        IPerformanceScope Measure(
            string operation);

        void Clear();
    }
}
