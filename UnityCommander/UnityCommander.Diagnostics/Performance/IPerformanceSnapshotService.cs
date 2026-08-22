
namespace UnityCommander.Diagnostics.Performance
{
    public interface IPerformanceSnapshotService
    {
        PerformanceSnapshot Create(string name);

        IReadOnlyList<PerformanceSnapshot> GetAll();

        PerformanceSnapshot? Get(string name);

        void Clear();
    }
}
