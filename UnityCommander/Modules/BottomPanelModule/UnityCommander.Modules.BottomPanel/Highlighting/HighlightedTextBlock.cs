
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using UnityCommander.Modules.BottomPanel.ViewModels;

namespace UnityCommander.Modules.BottomPanel.Highlighting
{
    public sealed class HighlightedTextBlock : TextBlock
    {
        public static readonly DependencyProperty PartsProperty =
            DependencyProperty.Register(
                nameof(Parts),
                typeof(IReadOnlyList<LogInline>),
                typeof(HighlightedTextBlock),
                new PropertyMetadata(null, OnPartsChanged));

        public IReadOnlyList<LogInline>? Parts
        {
            get => (IReadOnlyList<LogInline>?)GetValue(PartsProperty);
            set => SetValue(PartsProperty, value);
        }

        private static void OnPartsChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var control = (HighlightedTextBlock)d;

            control.Inlines.Clear();

            if (e.NewValue is not IReadOnlyList<LogInline> parts)
                return;

            foreach (var part in parts)
            {
                control.Inlines.Add(new Run(part.Text)
                {
                    Foreground = part.Foreground,
                    Background = part.Background
                });
            }
        }
    }
}
