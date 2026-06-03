
namespace UnityCommander.Commands.Parsing
{
    public interface IArgumentCollection
    {
        bool HasFlag(string name);

        string? GetString(string name);

        int GetInt(
            string name, 
            int defaultValue = 0);

        //double GetDouble(
        //    string name, 
        //    double defaultValue = 0);
    }
}
