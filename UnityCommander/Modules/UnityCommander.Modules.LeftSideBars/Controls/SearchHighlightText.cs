
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace UnityCommander.Modules.LeftSideBars.Controls
{
    public sealed class SearchHighlightText : TextBlock
    {
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                nameof(Text),
                typeof(string),
                typeof(SearchHighlightText),
                new FrameworkPropertyMetadata(
                    string.Empty,
                    FrameworkPropertyMetadataOptions.AffectsRender,
                    OnTextChanged));

        public static readonly DependencyProperty QueryProperty =
            DependencyProperty.Register(
                nameof(Query),
                typeof(string),
                typeof(SearchHighlightText),
                new FrameworkPropertyMetadata(
                    string.Empty,
                    FrameworkPropertyMetadataOptions.AffectsRender,
                    OnTextChanged));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public string Query
        {
            get => (string)GetValue(QueryProperty);
            set => SetValue(QueryProperty, value);
        }

        private static void OnTextChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            ((SearchHighlightText)d).Rebuild();
        }

        private void Rebuild()
        {
            Inlines.Clear();

            if (string.IsNullOrEmpty(Text))
                return;

            if (string.IsNullOrEmpty(Query))
            {
                Inlines.Add(new Run(Text));
                return;
            }

            var position = 0;

            while (position < Text.Length)
            {
                var index = Text.IndexOf(
                    Query,
                    position,
                    StringComparison.OrdinalIgnoreCase);

                if (index < 0)
                {
                    Inlines.Add(new Run(Text[position..]));
                    break;
                }

                if (index > position)
                {
                    Inlines.Add(
                        new Run(Text[position..index]));
                }

                Inlines.Add(
                    new Run(Text.Substring(index, Query.Length))
                    {
                        Background = (Brush)FindResource(
                            "Search.Highlight.Background"),
                        Foreground = (Brush)FindResource(
                            "Search.Highlight.Foreground")
                    });

                position = index + Query.Length;
            }
        }
    }
}
