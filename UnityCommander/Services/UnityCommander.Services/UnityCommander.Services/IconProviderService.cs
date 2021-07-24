
namespace UnityCommander.Services
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows.Media;
    using System.Windows.Shapes;

    using MaterialDesignThemes.Wpf;

    using UnityCommander.Common.Models.Icons;
    using UnityCommander.Services.Interfaces;

    using Icon = UnityCommander.Common.Models.Icons.Icon;

    /// <summary>
    /// The icon provider.
    /// </summary>
    public class IconProviderService : IIconProviderService
    {
        /// <summary>
        /// The material design icons.
        /// </summary>
        private readonly Dictionary<string, PackIconKind> materialDesignIcons = new Dictionary<string, PackIconKind>();

        /// <summary>
        /// Initializes a new instance of the <see cref="IconProviderService"/> class.
        /// </summary>
        public IconProviderService()
        {
            this.materialDesignIcons.Add("FileTree", PackIconKind.FileTree);
            this.materialDesignIcons.Add("TableColumn", PackIconKind.TableColumn);
            this.materialDesignIcons.Add("Tag", PackIconKind.Tag);
            this.materialDesignIcons.Add("Comment", PackIconKind.Comment);
            this.materialDesignIcons.Add("Plugin", PackIconKind.Plugin);
            this.materialDesignIcons.Add("Settings", PackIconKind.Settings);
        }

        /// <summary>
        /// TODO The get icon.
        /// </summary>
        /// <param name="iconName">
        /// TODO The icon name.
        /// </param>
        /// <returns>
        /// The <see cref="Common.Models.Icons.Icon"/>.
        /// </returns>
        public IIcon GetIcon(string iconName)
        {
            var iconModel = new Icon();
            PackIcon packIcon = new PackIcon { Kind = this.materialDesignIcons.Single(pack => pack.Key == iconName).Value };
            iconModel.Path = new Path
            {
                Data = Geometry.Parse(packIcon.Data)
            };

            return iconModel;
        }


        /// <summary>
        /// The get icons.
        /// </summary>
        /// <returns>
        /// The collection icons of type Icon.
        /// </returns>
        public ObservableCollection<IIcon> GetIcons()
        {
            var icons = new ObservableCollection<IIcon>();

            foreach (var icon in this.materialDesignIcons)
            {
                PackIcon packIcon = new PackIcon { Kind = icon.Value };
                icons.Add(new Icon
                              {
                                  Category = icon.Key,
                                  Path = new Path 
                                             { 
                                                 Data = Geometry.Parse(packIcon.Data)
                                             }
                              });
            }

            return icons;
        }
    }
}
