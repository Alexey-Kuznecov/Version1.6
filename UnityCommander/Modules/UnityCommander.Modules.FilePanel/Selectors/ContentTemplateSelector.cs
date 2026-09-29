
using System.Windows;
using System.Windows.Controls;
using UnityCommander.Common.Models;
using UnityCommander.Controls.Layout;
using UnityCommander.WPF;

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

        public DataTemplate UnifiedThumbnailTemplate
           => ResourceManager.Get<DataTemplate>("UnifiedThumbnailViewDataTemplate");

        public DataTemplate UnifiedTableTemplate
            => ResourceManager.Get<DataTemplate>("UnifiedItemViewDataTemplate");

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

                (ContentRole.Unified, ViewMode.Thumbnails)
                    => UnifiedThumbnailTemplate,

                (ContentRole.Unified, ViewMode.Table)
                    => UnifiedTableTemplate,

                //(ContentRole.Unified, ViewMode.Tiles)
                //    => UnifiedTilesTemplate,

                (ContentRole.Header, null)
                    => HeaderTemplate,

                (ContentRole.Drive, _)
                    => DashboardTemplate,

                _ => base.SelectTemplate(item, container)
            };
        }
    }
}
