
namespace UnityCommander.Diagnostics.Performance
{
    public interface IPerformanceScope : IDisposable
    {
        void SetMetadata(
            string key,
            object? value);
    }
}
