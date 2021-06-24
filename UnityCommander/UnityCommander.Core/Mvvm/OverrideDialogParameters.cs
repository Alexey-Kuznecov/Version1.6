
namespace UnityCommander.Core.Mvvm
{
    using Prism.Common;
    using Prism.Services.Dialogs;

    /// <summary>
    /// The dialog parameters.
    /// </summary>
    public class OverrideDialogParameters : ParametersBase, IDialogParameters
    {
        /// <summary>
        /// Gets or sets the package.
        /// </summary>
        public object Package { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OverrideDialogParameters"/> class.
        /// </summary>
        public OverrideDialogParameters()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OverrideDialogParameters"/> class.
        /// </summary>
        /// <param name="query"> Query string to be parsed. </param>
        public OverrideDialogParameters(string query)
            : base(query)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OverrideDialogParameters"/> class.
        /// </summary>
        /// <param name="obj">
        /// The query.
        /// </param>
        public OverrideDialogParameters(object obj)
            : base()
        {
            this.Package = obj;
        }
    }
}
