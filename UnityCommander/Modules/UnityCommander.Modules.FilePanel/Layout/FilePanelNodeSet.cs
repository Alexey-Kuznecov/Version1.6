
using UnityCommander.Controls.Layout;

namespace UnityCommander.Modules.FilePanel.Layout
{
    public sealed class FilePanelNodeSet
    {
        public ContentNode Folder { get; }
        public ContentNode File { get; }
        public ContentNode Drive { get; }
        public ContentNode Header { get; }
        public ContentNode Unified { get; }

        public FilePanelNodeSet(
            ContentNode folder,
            ContentNode file,
            ContentNode drive,
            ContentNode header,
            ContentNode unified)
        {
            Folder = folder;
            File = file;
            Drive = drive;
            Header = header;
            Unified = unified;
        }
    }
}
