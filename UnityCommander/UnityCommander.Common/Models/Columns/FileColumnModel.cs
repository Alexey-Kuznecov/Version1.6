
namespace UnityCommander.Common.Models.Columns
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// The file column model.
    /// </summary>
    public class FileColumnModel : DefaultColumns
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileColumnModel"/> class.
        /// </summary>
        public FileColumnModel()
        {
            this.InitialData();

            this.AddColumn(new BaseColumn
            {
                Header = "Extension",
                Template = new GridViewColumn
                {
                    Header = "Extension",
                    Width = 80,
                    CellTemplate = (DataTemplate)Application.Current.FindResource("ColumnExtensionDataTemplate")
                }
            });
        }
    }
}
