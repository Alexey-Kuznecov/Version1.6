
namespace UnityCommander.Integration.Contracts
{
    using System;
    using System.Collections.ObjectModel;
    using UnityCommander.Integration.Enums;

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
        /// Binds a column data model to a program user interface.
        /// </summary>
        /// <param name="model">
        /// The column model is bound to the target panel, that is specified as the second argument.
        /// </param>
        /// <param name="currentPath">
        /// The path of the current file or folder. 
        /// </param>
        void SetColumnValue(Action<object, TargetPanel> model, string currentPath);

        /// <summary>
        /// A method for two-way column data transfer between the plugin and the host.
        /// </summary>
        /// <param name="callback">
        /// Lambda expression. The first parameter is the column. The second possible plugin error..
        /// </param>
        void GetColumn(Action<ObservableCollection<IColumn>, Exception> callback);
    }
}
