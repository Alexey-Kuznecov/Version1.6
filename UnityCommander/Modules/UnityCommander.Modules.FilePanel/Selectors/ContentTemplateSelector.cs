
using System.Windows;
using System.Windows.Controls;
using UnityCommander.Common.Models;
using UnityCommander.Controls.Layout;
using UnityCommander.Core;
using UnityCommander.Core.Performance;

namespace UnityCommander.Modules.FilePanel.Selectors
{
    public class ContentTemplateSelector : DataTemplateSelector
    {
        public DataTemplate DirectoryTreeTemplate { get; set; }

        public DataTemplate DirectoryTableTemplate
            => ResourceManager.Get<DataTemplate>("DirectoryContentViewDataTemplate");

        public DataTemplate DirectoryTilesTemplate { get; set; }

        public DataTemplate FileTableTemplate 
            => ResourceManager.Get<DataTemplate>("FileContentViewDataTemplate");

        public DataTemplate FileThumbnailTemplate { get; set; }

        public DataTemplate HeaderTemplate
            => ResourceManager.Get<DataTemplate>("HeaderContentDataTemplate");

        public DataTemplate DashboardTemplate
             => ResourceManager.Get<DataTemplate>("DriveContentViewDataTemplate");

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
