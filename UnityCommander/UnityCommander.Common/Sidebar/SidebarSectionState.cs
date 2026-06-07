
namespace UnityCommander.Common.Sidebar
{
    public class SidebarSectionState : ISidebarSectionState
    {
        public string SectionId { get; set; }

        public bool IsActive { get; set; }

        public byte[]? Payload { get; set; }
    }
}
