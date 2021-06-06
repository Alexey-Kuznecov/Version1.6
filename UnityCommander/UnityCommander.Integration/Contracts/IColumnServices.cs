
namespace UnityCommander.Integration.Contracts
{
    using System;
    using System.Collections.ObjectModel;

    /// <summary>
    /// The <see langword="interface"/> for implementing custom plug-ins for directory panel columns.
    /// </summary>
    public interface IColumnService
    {
        /// <summary>
        /// Gets or sets the column title.
        /// </summary>
        string DisplayName { get; set; }

        /// <summary>
        /// The set column value.
        /// </summary>
        /// <param name="yourModel">
        /// The current path.
        /// </param>
        /// <param name="currentPath">
        /// The current Path.
        /// </param>
        void SetColumnValue(Action<object> yourModel, string currentPath);

        /// <summary>
        /// A method for two-way column data transfer between the plugin and the host.
        /// </summary>
        /// <param name="callback">
        /// Lambda expression. The first parameter is the column. The second possible plugin error..
        /// </param>
        void GetColumn(Action<ObservableCollection<IColumn>, Exception> callback);
    }
}
