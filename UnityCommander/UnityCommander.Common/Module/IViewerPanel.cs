
namespace UnityCommander.Common.Module
{
    /// <summary>
    /// The ViewerPanel interface.
    /// </summary>
    public interface IViewerPanel
    {        
        /// <summary>
        /// Gets or sets the current file path.
        /// </summary>
        public object ViewerContent { get; set; }

        public void SetViewerContent(object content);
    }
}
