
namespace UnityCommander.Integration.Contracts
{
    /// <summary>
    /// The selector type.
    /// </summary>
    public enum OptionRender
    {
        /// <summary>
        /// Renders the element as a drop-down list.
        /// Note the property must be <see cref="DropBoxModel"/> as a type.
        /// </summary>
        DropBox,

        /// <summary>
        /// Renders the element as a combo box.
        /// Note, the property must have an array of strings as the type.
        /// </summary>
        ComboBox,

        /// <summary>
        /// Renders the element as a text field.
        /// </summary>
        TextField,

        /// <summary>
        /// Renders the element as a simple text.
        /// </summary>
        TextBlock
    } 
}
