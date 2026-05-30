
namespace UnityCommander.Services
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows.Media;
    using System.Windows.Shapes;

    using MaterialDesignThemes.Wpf;
    using UnityCommander.Common.Commands;
    using UnityCommander.Common.Models.Icons;
    using UnityCommander.Services.Interfaces;

    using Icon = UnityCommander.Common.Models.Icons.Icon;

    /// <summary>
    /// The icon provider.
    /// </summary>
    public class PackIconProvider : IIconProviderService
    {
        private readonly Dictionary<string, PackIconKind> materialDesignIcons = new Dictionary<string, PackIconKind>();

        public PackIconProvider()
        {
            this.materialDesignIcons.Add("FileTree", PackIconKind.FileTree);
            this.materialDesignIcons.Add("TableColumn", PackIconKind.TableColumn);
            this.materialDesignIcons.Add("Tag", PackIconKind.Tag);
            this.materialDesignIcons.Add("Comment", PackIconKind.Comment);
            this.materialDesignIcons.Add("Plugin", PackIconKind.Plugin);
            this.materialDesignIcons.Add("Settings", PackIconKind.Settings);
            this.materialDesignIcons.Add(CommandNames.Navigation.Drives, PackIconKind.LaptopWindows);
            this.materialDesignIcons.Add(CommandNames.Navigation.Goto, PackIconKind.Arrow);
            this.materialDesignIcons.Add(CommandNames.Navigation.Refresh, PackIconKind.Refresh);
            this.materialDesignIcons.Add(CommandNames.Navigation.Back, PackIconKind.ArrowBack);
            this.materialDesignIcons.Add(CommandNames.Navigation.Forward, PackIconKind.ArrowForward);
        }

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
