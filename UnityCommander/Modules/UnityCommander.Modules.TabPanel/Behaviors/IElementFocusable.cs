
namespace UnityCommander.Modules.TabPanel.Behaviors
{
    /// <summary>
    /// The ElementFocusable interface.
    /// </summary>
    public interface IElementFocusable
    { 
        /// <summary>
        /// The focus element data provider.
        /// </summary>
        /// <param name="focusData">
        /// The focus data.
        /// </param>
        public void FocusElementDataProvider(ElementFocusData focusData);

        /// <summary>
        /// The focus element data provider.
        /// </summary>
        /// <param name="focusData">
        /// The focus data.
        /// </param>
        public void LastFocusElementDataProvider(ElementFocusData focusData);
    }
}
