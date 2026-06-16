
namespace UnityCommander.Core.Mvvm
{
    using Prism.Common;
    using Prism.Dialogs;

    public class OverrideDialogParameters : ParametersBase, IDialogParameters
    {
        public object Package { get; set; }

        public OverrideDialogParameters()
            : base()
        {
        }

        public OverrideDialogParameters(string query)
            : base(query)
        {
        }

        public OverrideDialogParameters(object obj)
            : base()
        {
            this.Package = obj;
        }

        public OverrideDialogParameters(CopyParameters parameters)
            : base()
        {
            this.Package = parameters;
        }
    }
}
