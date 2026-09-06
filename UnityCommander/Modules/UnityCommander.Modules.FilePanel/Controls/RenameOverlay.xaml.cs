
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace UnityCommander.Modules.FilePanel.Controls
{
    public partial class RenameOverlay : UserControl
    {
        public event Action? CancelRequested;

        public event Action<string>? RenameRequested;

        public RenameOverlay()
        {
            InitializeComponent();

            PreviewKeyDown += OnPreviewKeyDown; 
            Loaded += OnLoaded;
        }

        private void OnApply(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
            RenameRequested?.Invoke(NameTextBox.Text);
        }

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Collapsed;
            CancelRequested?.Invoke();
        }

        public void BeginEdit(string name, bool isFile)
        {
            NameTextBox.Text = name;

            Dispatcher.BeginInvoke(() =>
            {
                NameTextBox.Focus();
                Keyboard.Focus(NameTextBox);

                SelectName(name, isFile);
            }, DispatcherPriority.Input);
        }

        private void SelectName(string name, bool isFile)
        {
            if (!isFile)
            {
                NameTextBox.SelectAll();
                return;
            }

            var extension = Path.GetExtension(name);

            if (string.IsNullOrEmpty(extension))
            {
                NameTextBox.SelectAll();
                return;
            }

            NameTextBox.Select(
                0,
                name.Length - extension.Length);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            NameTextBox.Focus();
            Keyboard.Focus(NameTextBox);

            NameTextBox.SelectAll();
        }

        private void OnPreviewKeyDown(
           object sender,
           KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    Visibility = Visibility.Collapsed;
                    CancelRequested?.Invoke();
                    break;

                case Key.Enter:
                    Visibility = Visibility.Collapsed;
                    RenameRequested?.Invoke(NameTextBox.Text);
                    break;

                default:
                    return;
            }

            e.Handled = true;
        }
    }
}
