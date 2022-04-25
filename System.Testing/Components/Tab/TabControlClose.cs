using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Components.Tab
{
    /// <summary>
    /// The tab control close.
    /// </summary>
    public partial class TabControl
    {
        /// <summary>
        /// The value property.
        /// </summary>
        public static readonly DependencyProperty CloseCommandProperty =
            DependencyProperty.Register("CloseCommand", typeof(ICommand), typeof(TabControl), new PropertyMetadata(CloseCommandChangedCallback));

        /// <summary>
        /// The value property.
        /// </summary>
        public static readonly DependencyProperty CloseCommandParameterProperty =
            DependencyProperty.Register("CloseCommandParameter", typeof(object), typeof(TabControl), new PropertyMetadata(CloseCommandParameterChangedCallback));

        /// <summary>
        /// The value property.
        /// </summary>
        public static readonly DependencyProperty CloseContentProperty =
            DependencyProperty.Register("CloseContent", typeof(object), typeof(TabControl), new PropertyMetadata(CloseContentChangedCallback));

        /// <summary>
        /// The control tab.
        /// </summary>
        private RepeatButton closeControl;

        #region Setters and Getters Properties

        /// <summary>
        /// Gets or sets the command.
        /// </summary>
        public ICommand CloseCommand
        {
            get => (ICommand)this.GetValue(CloseCommandProperty);
            set => this.SetValue(CloseCommandProperty, value);
        }

        /// <summary>
        /// Gets or sets the command.
        /// </summary>
        public object CloseCommandParameter
        {
            get => this.GetValue(CloseCommandParameterProperty);
            set => this.SetValue(CloseCommandParameterProperty, value);
        }

        /// <summary>
        /// Gets or sets the command.
        /// </summary>
        public object CloseContent
        {
            get => this.GetValue(CloseContentProperty);
            set => this.SetValue(CloseContentProperty, value);
        }

        #endregion

        /// <summary>
        /// Gets or sets the control tab close.
        /// </summary>
        private RepeatButton CloseControl
        {
            get => this.closeControl;
            set
            {
                if (this.closeControl != null)
                {
                    this.closeControl.Click -= this.CloseControlClick;
                }

                this.closeControl = value;

                if (this.control != null)
                {
                    this.closeControl.Click += this.CloseControlClick;
                }
            }
        }

        /// <summary>
        /// The control tab_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void CloseControlClick(object sender, RoutedEventArgs e)
        {
            var paramerter = this.CloseCommandParameter ?? this.CommandParameter;

            if (paramerter == null)
            {
                paramerter = ((Control)sender).TemplatedParent;
            }
            
            this.CloseCommand.Execute(paramerter);
        }

        /// <summary>
        /// The close command changed callback.
        /// </summary>
        /// <param name="d">
        /// The d.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void CloseCommandChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// The close command parameter changed callback.
        /// </summary>
        /// <param name="d">
        /// The d.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void CloseCommandParameterChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// The close content changed callback.
        /// </summary>
        /// <param name="d">
        /// The d.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void CloseContentChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }
    }
}
