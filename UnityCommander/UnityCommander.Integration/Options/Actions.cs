
namespace UnityCommander.Integration.Options
{
    #region Options delegates

    /// <summary>
    /// The selector.
    /// </summary>
    /// <param name="selected">
    /// The selected.
    /// </param>
    public delegate void Selector(object selected);
    
    public delegate void Predictor(bool value);
    #endregion
}
