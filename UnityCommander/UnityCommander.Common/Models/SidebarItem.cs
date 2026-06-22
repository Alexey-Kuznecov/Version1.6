
namespace UnityCommander.Common.Models
{
    using System.Windows.Controls;

    public class SidebarItem
    {
        public string Id { get; set; } = default!;   // git.status

        public string Owner { get; set; } = default!; // git

        public UserControl Content { get; set; }

        public string IconKey { get; set; }

        public string? Title { get; set; }
    }
}
