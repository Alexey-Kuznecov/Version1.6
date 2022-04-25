
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
        /// Gets an icon provided by the material design library.
        /// </summary>
        /// <param name="icon">
        /// Specifies an icon from list of available icons.
        /// </param>
        /// <returns>
        /// Returns an icon of type <see cref="IIcon"/> that is known to the program.
        /// </returns>
        public IIcon GetIcon(PackIconKind icon)
        {
            var iconModel = new Icon();
            PackIcon packIcon = new PackIcon { Kind = icon };
            iconModel.Path = new Path
            {
                Data = Geometry.Parse(packIcon.Data)
            };

            return iconModel;
        }

        /// <summary>
        /// Provides an icon using the name specified in the arguments.
        /// </summary>
        /// <param name="iconName"> Icon name. </param>
        /// <returns> Icon as type of <see cref="IIcon"/>. </returns>
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
        /// Provides all icons is available.
        /// </summary>
        /// <returns>
        /// The collection icons of <see cref="IIcon"/> types.
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
