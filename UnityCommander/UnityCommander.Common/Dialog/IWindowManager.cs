
namespace UnityCommander.Common.Dialog
{
    /// <summary>
    /// The dialog service interface.
    /// </summary>
    public interface IWindowManager
    {
        bool ShowDialog(string id);

        bool? ShowModalDialog(string id);
    }
}
