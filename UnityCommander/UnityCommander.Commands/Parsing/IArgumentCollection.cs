
namespace UnityCommander.Commands.Parsing
{
    public interface IArgumentCollection
    {
        bool HasFlag(string name);

        string? GetString(string name);

        int GetInt(
            string name, 
            int defaultValue = 0);

        string GetAt(int index);
    }
}
