
namespace UnityCommander.Services
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Shapes;

    using MaterialDesignThemes.Wpf;
    using UnityCommander.Common.Models;
    using UnityCommander.Services.Interfaces;

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
            this.materialDesignIcons.Add("Column", PackIconKind.Column);
            this.materialDesignIcons.Add("Tag", PackIconKind.Tag);
            this.materialDesignIcons.Add("Message", PackIconKind.Message);
            this.materialDesignIcons.Add("Github", PackIconKind.Github);
        }

        /// <summary>
        /// The get icons.
        /// </summary>
        /// <returns>
        /// The collection icons of type IconModel.
        /// </returns>
        public ObservableCollection<IconModel> GetIcons()
        {
            var icons = new ObservableCollection<IconModel>();

            foreach (var icon in this.materialDesignIcons)
            {
                PackIcon packIcon = new PackIcon();
                packIcon.Kind = icon.Value;
                icons.Add(new IconModel
                {
                    Category = icon.Value.ToString(),
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
