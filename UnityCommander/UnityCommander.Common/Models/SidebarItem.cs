
namespace UnityCommander.Common.Models
{
    using System.Windows.Controls;

    using UnityCommander.Common.Models.Icons;

    public class SidebarItem
    {
        public string Id { get; set; } = default!;   // git.status

        public string Owner { get; set; } = default!; // git

        public UserControl Content { get; set; }

        public IIcon Icon { get; set; }

        public string? Title { get; set; }
    }
}
