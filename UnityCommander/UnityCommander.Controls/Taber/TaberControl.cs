
namespace UnityCommander.Controls.Taber
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;

    /// <summary>
    /// The taber control.
    /// </summary>
    [TemplatePart(Name = "ControlTab", Type = typeof(RepeatButton))]
    [TemplateVisualState(Name = "Positive", GroupName = "ValueStates")]
    [TemplateVisualState(Name = "Negative", GroupName = "ValueStates")]
    [TemplateVisualState(Name = "Focused", GroupName = "FocusedStates")]
    [TemplateVisualState(Name = "Unfocused", GroupName = "FocusedStates")]
    public class TaberControl : Control
    {
        /// <summary>
        /// The value property.
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(TaberControl), new PropertyMetadata(CommandChangedCallback));

        /// <summary>
        /// The value property.
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(TaberControl), new PropertyMetadata(CommandParameterChangedCallback));

        /// <summary>
        /// The value property.
        /// </summary>
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(object), typeof(TaberControl), new PropertyMetadata(ContentChangedCallback));

        /// <summary>
        /// The value changed event.
        /// </summary>
        public static readonly RoutedEvent ContentChangedEvent = 
            EventManager.RegisterRoutedEvent("ContentChanged", RoutingStrategy.Direct, typeof(ContentChangedEventHandler), typeof(TaberControl));

        /// <summary>
        /// The click tab event.
        /// </summary>
        public static readonly RoutedEvent TabClickEvent =
            EventManager.RegisterRoutedEvent("TabClick", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(TaberControl));

        private double width;

        /// <summary>
        /// The controlTab.
        /// </summary>
        private RepeatButton controlTab;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaberControl"/> class.
        /// </summary>
        public TaberControl()
        {
            this.DefaultStyleKey = typeof(TaberControl);
            this.IsTabStop = true;
        }

        /// <summary>
        /// The value changed.
        /// </summary>
        public event ContentChangedEventHandler ContentChanged
        {
            add => this.AddHandler(ContentChangedEvent, value);
            remove => this.RemoveHandler(ContentChangedEvent, value);
        }

        /// <summary>
        /// The value changed.
        /// </summary>
        public event RoutedEventHandler TabClick
        {
            add => this.AddHandler(TabClickEvent, value);
            remove => this.RemoveHandler(TabClickEvent, value);
        }

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
        /// Gets or sets a value indicating whether is selected tab.
        /// </summary>
        public bool IsSelectedTab { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is selected tab.
        /// </summary>
        public TaberControl CurrentTab { get; set; }
        
        public new double Width {
            get => this.width;
            set 
            {
                this.width = value;
                //this.ControlTab.Width = this.width;
            } 
        }

        /// <summary>
        /// Gets or sets the control tab.
        /// </summary>
        private RepeatButton ControlTab
        {
            get => this.controlTab;
            set
            {
                if (this.controlTab != null)
                {
                    this.controlTab.Click -= this.ControlTab_Click;
                }

                this.controlTab = value;

                if (this.controlTab != null)
                {
                    this.controlTab.Click += this.ControlTab_Click;
                }
            }
        }

        /// <summary>
        /// The on apply template.
        /// </summary>
        public override void OnApplyTemplate()
        {
            this.ControlTab = GetTemplateChild("ControlTab") as RepeatButton;

            if ((string)this.Content == "+")
            {
                this.ControlTab.Width = 50;
                this.ControlTab.FontSize = 18;
                this.ControlTab.FontWeight = FontWeights.Bold;
                this.ControlTab.Background = null;
                this.ControlTab.BorderBrush = null;
            }

            // TextElement = GetTemplateChild("TextBlock") as TextBlock;
            this.UpdateStates(false);
        }

        /// <summary>
        /// The on value changed.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected virtual void OnContentChanged(ContentChangedEventArgs e)
        {
            // Raise the ContentChanged event so applications can be alerted
            // when Command changes.
            this.RaiseEvent(e);
        }

        /// <summary>
        /// The on value changed.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected virtual void OnTabCommandExecuted(TabCommandExecutedEventArg e)
        {
            // Raise the ContentChanged event so applications can be alerted
            // when Command changes.
            this.RaiseEvent(e);
        }

        #region Event Handlers

        /// <summary>
        /// The control tab_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void ControlTab_Click(object sender, RoutedEventArgs e)
        {
            this.IsSelectedTab = true;
            this.Command.Execute(this.CommandParameter);

            var parent = this.Parent as TaberPanel;

            // Call OnContentChanged to raise the ContentChanged event.
            this.OnTabCommandExecuted(new TabCommandExecutedEventArg(TaberControl.TabClickEvent, parent));
        }

        /// <summary>
        /// The on mouse left controlTab down.
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

        #region Callback functions

        /// <summary>
        /// The value changed callback.
        /// </summary>
        /// <param name="d">
        /// The dependency object.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        private static void CommandChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            TaberControl control = (TaberControl)d;
            ICommand newValue = (ICommand)args.NewValue;

            // Call UpdateStates because the Command might have caused the
            // control to change ValueStates.
            if (newValue != null)
            {
                control.UpdateStates(true);
            }
            else
            {
                control.UpdateStates(false);
            } 
        }

        /// <summary>
        /// The value changed callback.
        /// </summary>
        /// <param name="d">
        /// The dependency object.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        private static void CommandParameterChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
        }

        /// <summary>
        /// The content changed callback.
        /// </summary>
        /// <param name="d">
        /// The d.
        /// </param>
        /// <param name="args">
        /// The e.
        /// </param>
        private static void ContentChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            TaberControl ctl = (TaberControl)d;

            // Call OnContentChanged to raise the ContentChanged event.
            ctl.OnContentChanged(new ContentChangedEventArgs(TaberControl.ContentChangedEvent, args.NewValue));
        }
        
        #endregion

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