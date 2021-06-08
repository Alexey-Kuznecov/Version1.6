
namespace ImagesColumns
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel.Composition;
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    
    using UnityCommander.Integration.Contracts;
    using UnityCommander.Integration.Enums;

    /// <summary>
    /// The home library book service.
    /// </summary>
    [Export(typeof(IColumnService))]
    public class Image : IColumnService
    {
        /// <summary>
        /// The columns.
        /// </summary>
        private ObservableCollection<IColumn> columns;

        /// <summary>
        /// Initializes a new instance of the <see cref="Image"/> class.
        /// </summary>
        public Image()
        {
           this.InitialData();
        }

        /// <summary>
        /// Gets or sets the column title.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the column title.
        /// </summary>
        public TargetPanel TargetType { get; set; }

        /// <summary>
        /// The get column.
        /// </summary>
        /// <param name="callback">
        /// The callback.
        /// </param>
        public void GetColumn(Action<ObservableCollection<IColumn>, Exception> callback)
        {
            callback(this.columns, null);
        }

        /// <summary>
        /// The set column value.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <param name="currentPath">
        /// The current path.
        /// </param>
        public void SetColumnValue(Action<object, TargetPanel> model, string currentPath)
        {
            ImageModel imageModel = new ImageModel();
            var dir = Path.GetFileName(currentPath);

            if (dir != null && dir.Contains("dot"))
            {
                imageModel.Dpi = "2dpi";
                imageModel.Sized = "100x50";
                imageModel.Colors = "Greed";
            }
            else
            {
                imageModel.Dpi = "72dpi";
                imageModel.Sized = "1200x880";
                imageModel.Colors = "Blue";
            }

            model(imageModel, TargetPanel.Files);
        }

        /// <summary>
        /// The initial data.
        /// </summary>
        private void InitialData()
        {
            this.columns = new ObservableCollection<IColumn>
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
