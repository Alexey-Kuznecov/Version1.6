
namespace UnityCommander.Abstractions.Command
{
    public class CommandDefinition : ICommandDefinition
    {
        public string? Id { get; set; }
        public Type? CommandType { get; set; }
        public string OwnerId { get; set; }
    }
}
