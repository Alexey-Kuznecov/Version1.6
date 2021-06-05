
namespace ImagesColumns
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel.Composition;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;

    using UnityCommander.Integration.Contracts;

    /// <summary>
    /// The home library book service.
    /// </summary>
    [Export(typeof(IColumnService))]
    public class Plugin : IColumnService
    {
        /// <summary>
        /// The columns.
        /// </summary>
        private ObservableCollection<IColumn> columns;

        /// <summary>
        /// Initializes a new instance of the <see cref="Plugin"/> class.
        /// </summary>
        public Plugin()
        {
           this.InitialData();
        }

        /// <summary>
        /// Gets or sets the column title.
        /// </summary>
        public string DisplayName { get; set; }

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
        /// The initial data.
        /// </summary>
        private void InitialData()
        {
            this.columns = new ObservableCollection<IColumn>
            {
                new ImageColumn
                  {
                      Header = "Dpi",
                      IsDisplayed = true,
                      Template = new GridViewColumn
                      {
                          Header = "Dpi",
                          Width = 100,
                          DisplayMemberBinding =
                              new Binding { Path = new PropertyPath("Dpi") }
                      }
                  },
                  new ImageColumn
                  {
                      Header = "Sized",
                      IsDisplayed = true,
                      Template = new GridViewColumn
                      {
                          Header = "Sized",
                          Width = 100,
                          DisplayMemberBinding =
                              new Binding { Path = new PropertyPath("Sized") }
                      }
                  },
                  new ImageColumn
                  {
                      Header = "Colors",
                      IsDisplayed = false,
                      Template = new GridViewColumn
                      {
                          Header = "Colors",
                          Width = 100,
                          DisplayMemberBinding =
                              new Binding { Path = new PropertyPath("Colors") }
                      }
                  }
            };
        }
    }
}
