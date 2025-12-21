
namespace UnityCommander.Controls.Ribbon.Control
{
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;
    using UnityCommander.Common.Commands;
    using UnityCommander.Common.Models.Icons;

    /// <summary>
    /// All controls inherit the necessary functionality from this type.
    /// </summary>
    public class RibbonControl : IRibbonControl
    {
        internal RibbonControl(
            [NotNull] string controlName,
            [NotNull] IIcon controlIcon,
            [NotNull] RibbonCommand controlCommand,
            [NotNull] string styleName,
            [NotNull] string templateName,
            string dataTemplate)
        {
            this.DataBinding = new DataBindingControl 
            {
                Content = controlName,
                GlobalCommand = controlCommand,
                Icon = controlIcon.GetIconPath()
            };

            this.Template = (ControlTemplate)Application.Current.FindResource(templateName);
            this.Style = (Style)Application.Current.FindResource(styleName);
            this.DataTemplate = (DataTemplate)Application.Current.TryFindResource(dataTemplate);
        }

        public ControlTemplate Template { get; set; }
        public DataBindingControl DataBinding { get; set; }
        public DataTemplate DataTemplate { get; set; }
        public Style Style { get; set; }
    }
}
