
namespace UnityCommander.Abstractions.Dialog
{
    /// <summary>
    /// The dialog service interface.
    /// </summary>
    public interface IWindowManager
    {
        bool ShowDialog(string id);

        bool? ShowModalDialog(string id);

        TDialogResult? ShowModalDialog<TDialogResult>(
           string key,
           object? parameter = null)
           where TDialogResult : IDialogResult;
    }
}
