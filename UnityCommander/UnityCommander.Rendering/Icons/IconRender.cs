
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using UnityCommander.Abstractions;

namespace UnityCommander.Rendering.Icons
{
    public class IconRender : Control
    {

        #region Dependency properties for icon rendering

        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register(
                nameof(Data),
                typeof(Geometry),
                typeof(IconRender));

        public Geometry Data
        {
            get => (Geometry)GetValue(DataProperty);
            private set => SetValue(DataProperty, value);
        }

        public static readonly DependencyProperty BrushProperty =
            DependencyProperty.Register(
                nameof(Brush),
                typeof(Brush),
                typeof(IconRender));

        public Brush Brush
        {
            get => (Brush)GetValue(BrushProperty);
            set => SetValue(BrushProperty, value);
        }

        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register(
                nameof(Stroke),
                typeof(Brush),
                typeof(IconRender));

        public Brush Stroke
        {
            get => (Brush)GetValue(StrokeProperty);
            set => SetValue(StrokeProperty, value);
        }

        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register(
                nameof(Fill),
                typeof(Brush),
                typeof(IconRender));

        public Brush Fill
        {
            get => (Brush)GetValue(FillProperty);
            set => SetValue(FillProperty, value);
        }

        public static readonly DependencyProperty StrokeThicknessProperty =
           DependencyProperty.Register(
               nameof(StrokeThickness),
               typeof(double),
               typeof(IconRender));

        public double StrokeThickness
        {
            get => (double)GetValue(StrokeThicknessProperty);
            set => SetValue(StrokeThicknessProperty, value);
        }

        public static readonly DependencyProperty DefaultBrushProperty =
            DependencyProperty.Register(
                nameof(DefaultBrush),
                typeof(Brush),
                typeof(IconRender));

        public Brush DefaultBrush
        {
            get => (Brush)GetValue(DefaultBrushProperty);
            set => SetValue(DefaultBrushProperty, value);
        }

        public static readonly DependencyProperty HoverBrushProperty =
            DependencyProperty.Register(
                nameof(HoverBrush),
                typeof(Brush),
                typeof(IconRender));

        public Brush HoverBrush
        {
            get => (Brush)GetValue(HoverBrushProperty);
            set => SetValue(HoverBrushProperty, value);
        }

        #endregion

        #region Command and CommandParameter properties

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(
                nameof(Command),
                typeof(ICommand),
                typeof(IconRender));

        public ICommand? Command
        {
            get => (ICommand?)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register(
                nameof(CommandParameter),
                typeof(object),
                typeof(IconRender));


        public object? CommandParameter
        {
            get => (object?)GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        public static readonly DependencyProperty KeyProperty =
            DependencyProperty.Register(
                nameof(Key),
                typeof(string),
                typeof(IconRender),
                new PropertyMetadata(null, UpdateIcon));

        public string Key
        {
            get => (string)GetValue(KeyProperty);
            set => SetValue(KeyProperty, value);
        }

        #endregion

        #region Series of properties that affect the icon rendering

        public static readonly DependencyProperty ToneProperty =
          DependencyProperty.Register(
              nameof(Tone),
              typeof(IconTone),
              typeof(IconRender),
              new PropertyMetadata(IconTone.Default, UpdateIcon));

        public IconTone Tone
        {
            get => (IconTone)GetValue(ToneProperty);
            set => SetValue(ToneProperty, value);
        }

        public static readonly DependencyProperty StateProperty =
            DependencyProperty.Register(
                nameof(State),
                typeof(VisualState),
                typeof(IconRender),
                new PropertyMetadata(VisualState.Normal, UpdateIcon));

        public VisualState State
        {
            get => (VisualState)GetValue(StateProperty);
            set => SetValue(StateProperty, value);
        }

        public static readonly DependencyProperty IconKindProperty =
            DependencyProperty.Register(
                nameof(IconKind),
                typeof(IconKind),
                typeof(IconRender),
                new PropertyMetadata(IconKind.Default, UpdateIcon));

        public IconKind IconKind
        {
            get => (IconKind)GetValue(IconKindProperty);
            set => SetValue(IconKindProperty, value);
        }

        public static readonly DependencyProperty RoleProperty =
             DependencyProperty.Register(
                 nameof(Role),
                 typeof(IconRole),
                 typeof(IconRender),
                 new PropertyMetadata(IconRole.Generic, UpdateIcon));

        public IconRole Role
        {
            get => (IconRole)GetValue(RoleProperty);
            set => SetValue(RoleProperty, value);
        }

        #endregion


        public static readonly DependencyProperty ViewBoxXProperty =
           DependencyProperty.Register(
               nameof(ViewBoxX),
               typeof(double),
               typeof(IconRender));

        public double ViewBoxX
        {
            get => (double)GetValue(ViewBoxXProperty);
            set => SetValue(ViewBoxXProperty, value);
        }

        public static readonly DependencyProperty ViewBoxYProperty =
           DependencyProperty.Register(
               nameof(ViewBoxY),
               typeof(double),
               typeof(IconRender));

        public double ViewBoxY
        {
            get => (double)GetValue(ViewBoxYProperty);
            set => SetValue(ViewBoxYProperty, value);
        }

        public static readonly DependencyProperty ViewBoxWidthProperty =
           DependencyProperty.Register(
               nameof(ViewBoxWidth),
               typeof(double),
               typeof(IconRender));

        public double ViewBoxWidth
        {
            get => (double)GetValue(ViewBoxWidthProperty);
            set => SetValue(ViewBoxWidthProperty, value);
        }

        public static readonly DependencyProperty ViewBoxHeightProperty =
           DependencyProperty.Register(
               nameof(ViewBoxHeight),
               typeof(double),
               typeof(IconRender));

        public double ViewBoxHeight
        {
            get => (double)GetValue(ViewBoxHeightProperty);
            set => SetValue(ViewBoxHeightProperty, value);
        }

        public static readonly DependencyProperty LayersProperty =
          DependencyProperty.Register(
              nameof(Layers),
              typeof(IReadOnlyList<IconRenderLayer>),
              typeof(IconRender),
              new PropertyMetadata(default(IReadOnlyList<IconRenderLayer>), UpdateIcon));

        public IReadOnlyList<IconRenderLayer> Layers
        {
            get => (IReadOnlyList<IconRenderLayer>)GetValue(LayersProperty);
            set => SetValue(LayersProperty, value);
        }

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

        //protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        //{
        //    base.OnMouseLeftButtonDown(e);

        //    if (Command?.CanExecute(CommandParameter) == true)
        //        Command.Execute(CommandParameter);
        //}

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            if (Command?.CanExecute(CommandParameter) == true)
                Command.Execute(CommandParameter);
        }

        //private static void OnKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    var control = (IconRender)d;
        //    var key = (string)e.NewValue;

        //    if (!IconHub.TryGet(key, out var result))
        //        return;

        //    control.Data = result.Geometry;
        //    control.DefaultBrush = IconHub.Resolve(control.IconKind, control.Role, control.Tone, control.State);
        //}

        private static void UpdateIcon(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var control = (IconRender)d;

            if (e.NewValue is string key)
            {
                if (!IconHub.TryGet(key, out var result))
                    return;

                control.Layers = result.Layers;
                control.ViewBoxWidth = result.ViewBoxWidth;
                control.ViewBoxHeight = result.ViewBoxHeight;
                control.ViewBoxX = result.ViewBoxX;
                control.ViewBoxY = result.ViewBoxY;
            }

            if (e.NewValue is IconKind || e.NewValue is IconRole)
            {
                control.DefaultBrush =
                    IconHub.Resolve(
                        control.IconKind,
                        control.Role,
                        control.Tone,
                        control.State);
            }
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);

            if (Tone != IconTone.Interactive)
                return;

            SetCurrentValue(BrushProperty, HoverBrush);
        }
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            if (Tone != IconTone.Interactive)
                return;

            SetCurrentValue(BrushProperty, DefaultBrush);
        }
    }
}
