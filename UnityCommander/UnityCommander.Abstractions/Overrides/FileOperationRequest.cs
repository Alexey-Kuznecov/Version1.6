
namespace UnityCommander.Abstractions.Overrides
{
    public class FileOperationRequest
    {
        public Guid OperationId = Guid.NewGuid();
        public List<string> Sources { get; set; }
            = new List<string>();
        public string? Target { get; set; }
        public bool ShowDialog { get; set; }
    }
}
