
namespace ImagesColumns
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.Composition;
    using System.IO;
    using System.Windows.Controls;
    using System.Windows.Data;

    using UnityCommander.Integration.Contracts;
    using UnityCommander.Integration.Contracts.Columns;
    using UnityCommander.Integration.Enums;

    /// <summary>
    /// The home library book service.
    /// </summary>
    [Export(typeof(IPluginImplements))]
    public class Plugin : IPluginImplements
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Plugin"/> class.
        /// </summary>
        public Plugin()
        {
            this.Register = new List<Type>();
            this.InitialData();
        }

        /// <summary>
        /// Gets or sets the columns.
        /// </summary>
        [AttachHandler(PluginScopes.Columns, typeof(PluginOptionHandler), nameof(IColumnService.GetColumns))]
        public static ObservableCollection<IColumn> Columns { get; set; }

        /// <summary>
        /// Gets or sets the column title.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the register.
        /// </summary>
        public List<Type> Register { get; set; }

        /// <summary>
        /// The register type.
        /// </summary>
        public void RegisterType()
        {
            this.Register.Add(typeof(ImageModel));
            this.Register.Add(typeof(ImageColumnModel));
        }

        /// <summary>
        /// The initial data.
        /// </summary>
        private void InitialData()
        {
            Columns = new ObservableCollection<IColumn>
            {
                new ImageColumnModel
                {
                      Header = nameof(ImageModel.Dpi),
                      IsDisplayed = true,
                      TargetPanel = TargetPanel.Files,
                      Template = new GridViewColumn
                      {
                          Header = nameof(ImageModel.Dpi),
                          Width = 50,
                          DisplayMemberBinding =
                              new Binding(nameof(ImageModel.Dpi))
                      }
                },
                new ImageColumnModel
                {
                      Header = nameof(ImageModel.Sized),
                      IsDisplayed = true,
                      TargetPanel = TargetPanel.Files,
                      Template = new GridViewColumn
                      {
                          Header = nameof(ImageModel.Sized),
                          Width = 80,
                          DisplayMemberBinding =
                              new Binding(nameof(ImageModel.Sized))
                      }
                  },
                  new ImageColumnModel
                  {
                      Header = nameof(ImageModel.Colors),
                      IsDisplayed = true,
                      TargetPanel = TargetPanel.Files,
                      Template = new GridViewColumn
                      {
                          Header = nameof(ImageModel.Colors),
                          Width = 60,
                          DisplayMemberBinding =
                              new Binding(nameof(ImageModel.Colors))
                      }
                  }
            };
        }
    }
}
