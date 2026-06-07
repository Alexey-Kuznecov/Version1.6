
namespace UnityCommander.Bootstrap
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using Prism.Dialogs;
    using Prism.Ioc;

    public class OverrideDialogService : DialogService
    {
        public static readonly DependencyProperty WindowTokenProperty =
            DependencyProperty.RegisterAttached("WindowToken", typeof(WindowToken), typeof(OverrideDialogService), new PropertyMetadata(null));

        private Dictionary<WindowToken, IDialogWindow> window = new Dictionary<WindowToken, IDialogWindow>();

        private WindowToken token = null;

        public OverrideDialogService(IContainerExtension containerExtension)
            : base(containerExtension)
        {
        }

        public static WindowToken GetWindowToken(DependencyObject dependencyObject)
        {
            return (WindowToken)dependencyObject.GetValue(WindowTokenProperty);
        }

        public static void SetWindowToken(DependencyObject dependencyObject, WindowToken value)
        {
            dependencyObject.SetValue(WindowTokenProperty, value);
        }

        internal WindowToken ShowManaged(string name, IDialogParameters parameters, DialogCallback callback)
        {
            ShowDialog(name, parameters, callback);
            return token;
        }

        internal void Activate(WindowToken token)
        {
            var dialogWindow = window[token];

            if (dialogWindow is Window w)
            {
                w.Activate();
            }
        }

        internal void Close(WindowToken token)
        {
            var dialogWindow = window[token];
            dialogWindow.Close();
        }

        protected override void ConfigureDialogWindowEvents(
             IDialogWindow dialogWindow,
             DialogCallback callback)
        {
            base.ConfigureDialogWindowEvents(dialogWindow, callback);

            EventHandler tokeCleanup = null;

            tokeCleanup = (s, e) =>
            {
                dialogWindow.Closed -= tokeCleanup;
                var token = GetWindowToken((DependencyObject)dialogWindow);
                window.Remove(token);
            };
            dialogWindow.Closed += tokeCleanup;
        }

        protected override IDialogWindow CreateDialogWindow(string name)
        {
            var dialogWindow = base.CreateDialogWindow(name);

            if (dialogWindow is Window window)
            {
            }

            token = new WindowToken();

            SetWindowToken((DependencyObject)dialogWindow, token);

            this.window.Add(token, dialogWindow);

            return dialogWindow;
        }
    }
}
