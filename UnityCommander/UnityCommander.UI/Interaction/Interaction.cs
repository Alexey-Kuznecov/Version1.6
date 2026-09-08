
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;

namespace UnityCommander.UI.Interaction
{
    public static class Interaction
    {
        private static IContextActionProvider? _actionProvider;

        public static readonly DependencyProperty ContextSourceProperty =
            DependencyProperty.RegisterAttached(
                "ContextSource",
                typeof(object),
                typeof(Interaction),
                new PropertyMetadata(null, OnContextSourceChanged));

        public static void SetContextSource(
            DependencyObject element,
            object? value)
        {
            element.SetValue(ContextSourceProperty, value);
        }

        public static object? GetContextSource(
            DependencyObject element)
        {
            return element.GetValue(ContextSourceProperty);
        }


        private static readonly DependencyProperty ContextProperty =
            DependencyProperty.RegisterAttached(
                "Context",
                typeof(IInteractionContext),
                typeof(Interaction),
                new PropertyMetadata(null));

        public static IInteractionContext? GetContext(
            DependencyObject element)
        {
            return (IInteractionContext?)element.GetValue(ContextProperty);
        }

        private static void SetContext(
            DependencyObject element,
            IInteractionContext? value)
        {
            element.SetValue(ContextProperty, value);
        }


        public static readonly DependencyProperty ActionsProperty =
            DependencyProperty.RegisterAttached(
                "Actions",
                typeof(IReadOnlyList<ContextActionItem>),
                typeof(Interaction),
                new PropertyMetadata(null));

        public static IReadOnlyList<ContextActionItem>? GetActions(
            DependencyObject element)
        {
            return (IReadOnlyList<ContextActionItem>?)element.GetValue(ActionsProperty);
        }

        private static void SetActions(
            DependencyObject element,
            IReadOnlyList<ContextActionItem>? value)
        {
            element.SetValue(ActionsProperty, value);
        }


        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.RegisterAttached(
                "IsOpen",
                typeof(bool),
                typeof(Interaction),
                new PropertyMetadata(false));

        public static bool GetIsOpen(DependencyObject element)
            => (bool)element.GetValue(IsOpenProperty);

        private static void SetIsOpen(
            DependencyObject element,
            bool value)
            => element.SetValue(IsOpenProperty, value);


        public static readonly DependencyProperty PopupHostProperty =
            DependencyProperty.RegisterAttached(
                "PopupHost",
                typeof(bool),
                typeof(Interaction),
                new PropertyMetadata(false, OnPopupHostChanged));

        public static bool GetPopupHost(DependencyObject element)
            => (bool)element.GetValue(PopupHostProperty);

        public static void SetPopupHost(
            DependencyObject element,
            bool value)
            => element.SetValue(PopupHostProperty, value);


        private static readonly DependencyProperty StateProperty =
            DependencyProperty.RegisterAttached(
                "State",
                typeof(InteractionState),
                typeof(Interaction),
                new PropertyMetadata(null));

        private static InteractionState GetState(
            DependencyObject element)
        {
            var state =
                (InteractionState?)element.GetValue(StateProperty);

            if (state != null)
                return state;

            state = new InteractionState();

            element.SetValue(StateProperty, state);

            return state;
        }


        private static void OnContextSourceChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            if (d is not FrameworkElement element)
                return;

            element.MouseEnter -= OnMouseEnter;
            element.MouseLeave -= OnMouseLeave;

            if (e.NewValue is not IInteractionContextSource source)
            {
                SetContext(element, null);
                SetActions(element, null);

                var state = GetState(element);

                state.TargetHovered = false;
                state.PopupHovered = false;

                SetIsOpen(element, false);

                return;
            }

            SetContext(element, source.GetContext());

            element.MouseEnter += OnMouseEnter;
            element.MouseLeave += OnMouseLeave;
        }

        private static void OnMouseEnter(
          object sender,
          MouseEventArgs e)
        {
            if (sender is not DependencyObject element)
                return;

            var context = GetContext(element);

            if (context is null)
                return;

            var state = GetState(element);

            state.TargetHovered = true;
            state.CancelClose();

            state.OpenTimer ??= CreateOpenTimer(element);

            state.OpenTimer.Stop();
            state.OpenTimer.Start();
        }

        private static void OnMouseLeave(
           object sender,
           MouseEventArgs e)
        {
            if (sender is not DependencyObject element)
                return;

            var state = GetState(element);

            state.TargetHovered = false;

            state.OpenTimer?.Stop();

            state.ScheduleClose(element);
        }


        public static void Initialize(
            IContextActionProvider contextAction)
        {
            _actionProvider ??= contextAction;
        }

        private static void OnPopupHostChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            if (d is not Popup popup)
                return;

            popup.MouseEnter -= OnPopupMouseEnter;
            popup.MouseLeave -= OnPopupMouseLeave;

            if (e.NewValue is true)
            {
                popup.MouseEnter += OnPopupMouseEnter;
                popup.MouseLeave += OnPopupMouseLeave;
            }
        }

        private static void OnPopupMouseEnter(
            object sender,
            MouseEventArgs e)
        {
            if (sender is not Popup popup)
                return;

            if (popup.PlacementTarget is not DependencyObject target)
                return;

            var state = GetState(target);

            state.PopupHovered = true;
            state.CancelClose();

            SetIsOpen(target, true);
        }


        private static void OnPopupMouseLeave(
          object sender,
          MouseEventArgs e)
        {
            if (sender is not Popup popup)
                return;

            if (popup.PlacementTarget is not DependencyObject target)
                return;

            var state = GetState(target);

            state.PopupHovered = false;

            state.ScheduleClose(target);
        }

        private static DispatcherTimer CreateOpenTimer(
         DependencyObject element)
        {
            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(800)
            };

            timer.Tick += (_, _) =>
            {
                timer.Stop();

                var state = GetState(element);

                if (!state.TargetHovered || state.PopupHovered)
                    return;

                var context = GetContext(element);

                if (context is null)
                    return;

                var actions = _actionProvider?
                    .GetActions(context)?
                    .Where(x => x.CanExecute(context))
                    .Select(x => new ContextActionItem(x, context))
                    .ToList();

                if (actions is null || actions.Count == 0)
                    return;

                SetActions(element, actions);
                SetIsOpen(element, true);
            };

            return timer;
        }

        private sealed class InteractionState
        {
            private DependencyObject? _target;

            public bool TargetHovered { get; set; }
            public bool PopupHovered { get; set; }

            public bool IsOpen =>
                TargetHovered || PopupHovered;

            public DispatcherTimer? OpenTimer { get; set; }
            public DispatcherTimer? CloseTimer { get; set; }

            public void ScheduleClose(DependencyObject target)
            {
                _target = target;

                OpenTimer?.Stop();

                CloseTimer ??= CreateCloseTimer();
                CloseTimer.Stop();
                CloseTimer.Start();
            }

            public void CancelClose()
            {
                CloseTimer?.Stop();
            }

            private DispatcherTimer CreateCloseTimer()
            {
                var timer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromMilliseconds(150)
                };

                timer.Tick += (_, _) =>
                {
                    timer.Stop();

                    if (TargetHovered || PopupHovered)
                        return;

                    if (_target is not null)
                        Interaction.SetIsOpen(_target, false);
                };

                return timer;
            }
        }
    }
}
