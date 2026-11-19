
namespace UnityCommander.Preview.Abstraction.Interfaces
{
    public interface IPreviewStateProvider
    {
        string StateId { get; }

        object CaptureState();

        void RestoreState(string state);
    }
}
