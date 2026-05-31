
using System.Windows;
using System.Windows.Controls;
using UnityCommander.Common.Models;
using UnityCommander.Controls.Layout;

namespace UnityCommander.Modules.FilePanel.Selectors
{
    public class ContentTemplateSelector : DataTemplateSelector
    {
        public DataTemplate DirectoryTreeTemplate { get; set; }

        public DataTemplate DirectoryTableTemplate =>
            (DataTemplate)Application.Current.TryFindResource("DirectoryContentViewDataTemplate");

        public DataTemplate DirectoryTilesTemplate { get; set; }

        public DataTemplate FileTableTemplate =>
            (DataTemplate)Application.Current.TryFindResource("FileContentViewDataTemplate");

        public DataTemplate FileThumbnailTemplate { get; set; }

        public DataTemplate HeaderTemplate =>
          (DataTemplate)Application.Current.TryFindResource("HeaderContentDataTemplate");

        public DataTemplate DashboardTemplate =>
            (DataTemplate) Application.Current.TryFindResource("DashboardContentViewDataTemplate");

        public override DataTemplate SelectTemplate(
            object item,
            DependencyObject container)
        {
            if (item is not ContentNode node)
                return base.SelectTemplate(item, container);

            return (node.Role, node.ViewMode) switch
            {
                (ContentRole.Directory, ViewMode.Table)
                    => DirectoryTableTemplate,

                (ContentRole.Directory, ViewMode.Tiles)
                    => DirectoryTilesTemplate,

                (ContentRole.File, ViewMode.Table)
                    => FileTableTemplate,

                (ContentRole.File, ViewMode.Thumbnails)
                    => FileThumbnailTemplate,

                (ContentRole.Header, null)
                    => HeaderTemplate,

                (ContentRole.Drive, _)
                    => DashboardTemplate,

                _ => base.SelectTemplate(item, container)
            };
        }
    }
}
