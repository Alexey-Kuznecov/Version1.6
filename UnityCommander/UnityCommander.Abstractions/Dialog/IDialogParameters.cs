namespace UnityCommander.Abstractions.Dialog
{
    public interface IDialogParameters
    {
        T GetValue<T>(string key);

        bool TryGetValue<T>(
            string key,
            out T value);
    }
}