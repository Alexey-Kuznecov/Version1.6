
namespace UnityCommander.Diagnostics.Abstraction.Performance
{
    public interface IPerformanceScope : IDisposable
    {
        void SetMetadata(
            string key,
            object? value);
    }
}
