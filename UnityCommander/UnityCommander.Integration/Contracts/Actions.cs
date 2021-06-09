
namespace UnityCommander.Integration.Contracts
{
    /// <summary>
    /// The insert data column.
    /// </summary>
    /// <param name="path">
    /// The path.
    /// </param>
    /// <returns> The objects. </returns>
    public delegate object InsertColumnData(string path);

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



}