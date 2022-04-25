
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Components.Tab
{
    /// <summary>
    /// The tab add control.
    /// </summary>
    [TemplatePart(Name = "Control", Type = typeof(RepeatButton))]
    [TemplateVisualState(Name = "Positive", GroupName = "ValueStates")]
    [TemplateVisualState(Name = "Negative", GroupName = "ValueStates")]
    [TemplateVisualState(Name = "Focused", GroupName = "FocusedStates")]
    [TemplateVisualState(Name = "Unfocused", GroupName = "FocusedStates")]
    public class AddTabControl : Control
    {
        /// <summary>
        /// The value property.
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(AddTabControl), new PropertyMetadata(CommandChangedCallback));

        /// <summary>
        /// The value property.
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(AddTabControl), new PropertyMetadata(CommandParameterChangedCallback));

        /// <summary>
        /// The value property.
        /// </summary>
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(object), typeof(AddTabControl), new PropertyMetadata(ContentChangedCallback));

        /// <summary>
        /// The control tab.
        /// </summary>
        private RepeatButton addControl;

        #region Setters and Getters Properties

        /// <summary>
        /// Gets or sets the command.
        /// </summary>
        public ICommand Command
        {
            get => (ICommand)this.GetValue(CommandProperty);
            set => this.SetValue(CommandProperty, value);
        }

        /// <summary>
        /// Gets or sets the command.
        /// </summary>
        public object CommandParameter
        {
            get => this.GetValue(CommandParameterProperty);
            set => this.SetValue(CommandParameterProperty, value);
        }

        /// <summary>
        /// Gets or sets the command.
        /// </summary>
        public object Content
        {
            get => this.GetValue(ContentProperty);
            set => this.SetValue(ContentProperty, value);
        }

        #endregion

        /// <summary>
        /// Gets or sets the control tab .
        /// </summary>
        private RepeatButton AddControl
        {
            get => this.addControl;
            set
            {
                if (this.addControl != null)
                {
                    this.addControl.Click -= this.AddControlClick;
                }

                this.addControl = value;

                if (this.addControl != null)
                {
                    this.addControl.Click += this.AddControlClick;
                }
            }
        }

        /// <summary>
        /// The on apply template.
        /// </summary>
        public override void OnApplyTemplate()
        {
            this.AddControl = GetTemplateChild("AddTabControl") as RepeatButton;
            this.UpdateStates(false);
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
        protected void AddControlClick(object sender, RoutedEventArgs e)
        {
            var paramerter = this.CommandParameter;

            if (paramerter == null)
            {
                var tabControl = (((Control)sender).TemplatedParent) as AddTabControl;
                paramerter = tabControl.Parent;
            }

            this.Command.Execute(paramerter);
        }

        #region Event Handlers

        /// <summary>
        /// The on mouse left control down.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            this.Focus();
        }

        /// <summary>
        /// The on got focus.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            this.UpdateStates(true);
        }

        /// <summary>
        /// The on lost focus.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            this.UpdateStates(true);
        }

        #endregion

        /// <summary>
        /// The  command changed callback.
        /// </summary>
        /// <param name="d">
        /// The d.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void CommandChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// The  command parameter changed callback.
        /// </summary>
        /// <param name="d">
        /// The d.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void CommandParameterChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// The  content changed callback.
        /// </summary>
        /// <param name="d">
        /// The d.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void ContentChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// The update states.
        /// </summary>
        /// <param name="useTransitions">
        /// The use transitions.
        /// </param>
        private void UpdateStates(bool useTransitions)
        {
            VisualStateManager.GoToState(this, this.Command != null ? "Positive" : "Negative", useTransitions);
            VisualStateManager.GoToState(this, this.IsFocused ? "Focused" : "Unfocused", useTransitions);
        }
    }
}
