
namespace UnityCommander.Modules.FilePanel.Selectors
{
    using System.Windows;
    using System.Windows.Controls;

    using UnityCommander.Common.Models.Directory;

    /// <summary>
    /// The column data template selector.
    /// </summary>
    public class ColumnDataTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// The select template.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <param name="container">
        /// The container.
        /// </param>
        /// <returns>
        /// The <see cref="DataTemplate"/>.
        /// </returns>
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            DataTemplate dataTemplate = null;

            FrameworkElement element = container as FrameworkElement;

            if (element != null && item != null && item is FileModel)
            {
                dataTemplate = element.FindResource("ColumnNameDataTemplate") as DataTemplate;
                return dataTemplate;
            }
            else
            {
                if (element != null)
                {
                    dataTemplate = element.FindResource("ColumnNameDataTemplate") as DataTemplate;
                }

                return dataTemplate;
            }
        }
    }
}
