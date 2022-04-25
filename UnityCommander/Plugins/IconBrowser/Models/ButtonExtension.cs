
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace AIconBrowser.Models
{
    /// <summary>
    /// The button extension.
    /// </summary>
    [Serializable]
    public class ButtonExtension
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public ushort Id { get; set; }

        /// <summary>
        /// Gets or sets the brush.
        /// </summary>
        public DrawingBrush Brush { get; set; }

        /// <summary>
        /// Gets the command parameter.
        /// </summary>
        public ButtonExtension CommandParameter { get; internal set; }

        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        public SolidColorBrush Color { get; set; }

        /// <summary>
        /// Gets or sets the style.
        /// </summary>
        public Style Style { get; set; }

        /// <summary>
        /// Gets the path.
        /// </summary>
        public Path Path { get; internal set; }

        /// <summary>
        /// Gets or sets the collection name.
        /// </summary>
        public string CollectionName { get; set; }

        /// <summary>
        /// Gets or sets the icon name.
        /// </summary>
        public string IconName { get; set; }

        /// <summary>
        /// Gets or sets the tool tip.
        /// </summary>
        public object ToolTip { get; set; }

        /// <summary>
        /// Gets or sets the template.
        /// </summary>
        public object Template { get; set; }
        
        /// <summary>
        /// Комманда для удаления икнок из редактора.
        /// </summary>       
        public ICommand RemoveIcon { get; set; }

        /// <summary>
        /// Gets or sets the rename icon.
        /// </summary>
        public ICommand RenameIcon { get; set; }

        /// <summary>
        /// Gets or sets the replace icon.
        /// </summary>
        public ICommand ReplaceIcon { get; set; }
    }
}
