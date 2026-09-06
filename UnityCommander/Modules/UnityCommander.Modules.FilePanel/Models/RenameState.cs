
namespace UnityCommander.Modules.FilePanel.Models
{
    public sealed class RenameState
    {
        public bool IsActive { get; private set; }

        public string? SourcePath { get; private set; }

        public void Start(string sourcePath)
        {
            SourcePath = sourcePath;
            IsActive = true;
        }

        public void Reset()
        {
            SourcePath = null;
            IsActive = false;
        }
    }
}
