
namespace UnityCommander.Integration.Contracts
{
    /// <summary>
    /// The option handler.
    /// TODO: Add description here.
    /// </summary>
    /// <param name="path">
    /// The path.
    /// </param>
    /// <param name="selected">
    /// The selected.
    /// </param>
    /// <returns>
    /// The object
    /// </returns>
    public delegate string OptionHandler(string path, string selected);

    /// <summary>
    /// The insert value.
    /// </summary>
    /// <param name="path">
    /// The path.
    /// </param>
    /// <returns>
    /// The object
    /// </returns>
    public delegate object InsertValueUsePath(string path);

    /// <summary>
    /// The get columns.
    /// </summary>
    /// <returns>
    /// The columns
    /// </returns>
    public delegate object AddColumnsDelegate();
}