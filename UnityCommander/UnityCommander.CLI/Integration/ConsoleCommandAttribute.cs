
namespace UnityCommander.CLI.Integration
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class ConsoleCommandAttribute : Attribute
    {
        public string Name { get; }
        public string Description { get; }
        public string[] Aliases { get; }

        public ConsoleCommandAttribute(string name, string description, params string[] aliases)
        {
            Name = name;
            Description = description;
            Aliases = aliases ?? Array.Empty<string>();
        }
    }
}
