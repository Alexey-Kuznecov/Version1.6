
namespace UnityCommander.Integration.Contracts.Columns
{
    /// <summary>
    /// The ColumnsService interface.
    /// </summary>
    public interface IColumnService
    {
        /// <summary>
        /// Binds a column data model to a program user interface.
        /// </summary>
        /// <param name="currentPath">
        /// The path of the current file or folder. 
        /// </param>
        /// <returns>
        /// The column values
        /// </returns>
        object GetColumnValues(string currentPath);

        /// <summary>
        /// A method for two-way column data transfer between the plugin and the host.
        /// </summary>
        /// <returns>
        /// The column.
        /// </returns>
        object GetColumns();
    }
}
