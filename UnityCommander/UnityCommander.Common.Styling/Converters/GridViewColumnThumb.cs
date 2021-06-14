
namespace UnityCommander.Common.Styling.Converters
{
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;

    /// <summary>
    /// The grid view column thumb.
    /// </summary>
    internal class GridViewColumnThumb : Thumb
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GridViewColumnThumb"/> class.
        /// </summary>
        public GridViewColumnThumb()
            : base()
        {
        }

        /// <summary>
        /// The on mouse left button down.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.SizeWE;
            base.OnMouseLeftButtonDown(e);
        }

        /// <summary>
        /// The on mouse left button up.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            Mouse.OverrideCursor = null;
        }
    }
}
