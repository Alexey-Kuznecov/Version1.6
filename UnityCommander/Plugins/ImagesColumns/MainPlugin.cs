
namespace ImagesColumns
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel.Composition;
    
    using UnityCommander.Integration.Contracts;

    /// <summary>
    /// The home library book service.
    /// </summary>
    [Export(typeof(IColumnService))]
    public class MainPlugin : IColumnService
    {
        /// <summary>
        /// The columns.
        /// </summary>
        private ObservableCollection<IColumn> columns;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainPlugin"/> class.
        /// </summary>
        public MainPlugin()
        {
           // this.InitialData();
        }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Title { get; set; } = "ImageColumn";

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
        //private void InitialData()
        //{
        //    this.columns = new ObservableCollection<IColumn>
        //    {
        //        new Column
        //          {
        //              Header = "Dpi",
        //              Template = new GridViewColumn
        //              {
        //                  Header = "Dpi",
        //                  Width = 100,
        //                  DisplayMemberBinding =
        //                      new Binding { Path = new PropertyPath("Dpi") }
        //              }
        //          },
        //          new Column
        //          {
        //              Header = "Sized",
        //              Template = new GridViewColumn
        //              {
        //                  Header = "Sized",
        //                  Width = 100,
        //                  DisplayMemberBinding =
        //                      new Binding { Path = new PropertyPath("Sized") }
        //              }
        //          },
        //          new Column
        //          {
        //              Header = "Colors",
        //              Template = new GridViewColumn
        //              {
        //                  Header = "Colors",
        //                  Width = 100,
        //                  DisplayMemberBinding =
        //                      new Binding { Path = new PropertyPath("Colors") }
        //              }
        //          }
        //    };
        //}
    }
}
