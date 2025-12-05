
using System.Windows;
using System.Windows.Controls;

namespace UnityCommander.Modules.FilePanel.Columns
{
    public class FileColumnModel : DefaultColumns
    {
        public FileColumnModel()
        {
            this.InitialData();

            this.AddColumn(new CommonColumn
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
