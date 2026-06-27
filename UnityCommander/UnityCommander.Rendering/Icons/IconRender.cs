
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace UnityCommander.Rendering.Icons
{
    public class IconRender : Control
    {
        public static readonly DependencyProperty KeyProperty =
            DependencyProperty.Register(
                nameof(Key),
                typeof(string),
                typeof(IconRender),
                new PropertyMetadata(null, OnKeyChanged));

        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register(
                nameof(Data),
                typeof(Geometry),
                typeof(IconRender));

        public static readonly DependencyProperty BrushProperty =
            DependencyProperty.Register(
                nameof(Brush),
                typeof(Brush),
                typeof(IconRender));

        public static readonly DependencyProperty DefaultBrushProperty =
            DependencyProperty.Register(
                nameof(DefaultBrush),
                typeof(Brush),
                typeof(IconRender));

        //public static readonly DependencyProperty HoverBrushProperty =
        //    DependencyProperty.Register(
        //        nameof(HoverBrush),
        //        typeof(Brush),
        //        typeof(IconRender));

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(
                nameof(Command),
                typeof(ICommand),
                typeof(IconRender));

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register(
                nameof(CommandParameter),
                typeof(object),
                typeof(IconRender));

        public static readonly DependencyProperty ToneProperty =
            DependencyProperty.Register(
                nameof(Tone),
                typeof(IconTone),
                typeof(IconRender),
                new PropertyMetadata(IconTone.Default));

        public static readonly DependencyProperty StateProperty =
            DependencyProperty.Register(
                nameof(State),
                typeof(VisualState),
                typeof(IconRender),
                new PropertyMetadata(VisualState.Normal));

        public static readonly DependencyProperty RoleProperty =
            DependencyProperty.Register(
                nameof(Role),
                typeof(IconRole),
                typeof(IconRender),
                new PropertyMetadata(IconRole.Generic));

        public ICommand? Command
        {
            get => (ICommand?)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public object? CommandParameter
        {
            get => (object?)GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        public IconTone Tone
        {
            get => (IconTone)GetValue(ToneProperty);
            set => SetValue(ToneProperty, value);
        }

        public VisualState State
        {
            get => (VisualState)GetValue(StateProperty);
            set => SetValue(StateProperty, value);
        }

        public IconRole Role
        {
            get => (IconRole)GetValue(RoleProperty);
            set => SetValue(RoleProperty, value);
        }

        public string Key
        {
            get => (string)GetValue(KeyProperty);
            set => SetValue(KeyProperty, value);
        }

        public Geometry Data
        {
            get => (Geometry)GetValue(DataProperty);
            private set => SetValue(DataProperty, value);
        }

        public Brush Brush
        {
            get => (Brush)GetValue(BrushProperty);
            set => SetValue(BrushProperty, value);
        }

        public Brush DefaultBrush
        {
            get => (Brush)GetValue(DefaultBrushProperty);
            set => SetValue(DefaultBrushProperty, value);
        }

        //public Brush HoverBrush
        //{
        //    get => (Brush)GetValue(HoverBrushProperty);
        //    set => SetValue(HoverBrushProperty, value);
        //}

        static IconRender()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(IconRender),
                new FrameworkPropertyMetadata(typeof(IconRender)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            if (Command?.CanExecute(CommandParameter) == true)
                Command.Execute(CommandParameter);
        }

        private static void OnKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (IconRender)d;
            var key = (string)e.NewValue;

            if (!IconHub.TryGet(key, out var result))
                return;

            control.Data = result.Geometry;
            control.DefaultBrush = IconHub.Resolve(control.Role, control.Tone, control.State);
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);

            if (Tone != IconTone.Interactive)
                return;

            //SetCurrentValue(BrushProperty, HoverBrush);
        }
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            if (Tone != IconTone.Interactive)
                return;

            //SetCurrentValue(BrushProperty, DefaultBrush);
        }
    }
}
