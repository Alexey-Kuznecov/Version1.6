
namespace UnityCommander
{
    using System;
    using System.Collections.Generic;
    using System.Windows;

    using Prism.Ioc;
    using Prism.Services.Dialogs;

    /// <summary>
    /// 
    /// </summary>
    public class OverrideDialogService : DialogService
    {
        /// <summary>
        /// Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty WindowTokenProperty =
            DependencyProperty.RegisterAttached("WindowToken", typeof(WindowToken), typeof(OverrideDialogService), new PropertyMetadata(null));

        /// <summary>
        /// The window.
        /// </summary>
        private Dictionary<WindowToken, IDialogWindow> window = new Dictionary<WindowToken, IDialogWindow>();

        /// <summary>
        /// The token.
        /// </summary>
        private WindowToken token = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="OverrideDialogService"/> class.
        /// </summary>
        /// <param name="containerExtension">
        /// The container extension.
        /// </param>
        public OverrideDialogService(IContainerExtension containerExtension)
            : base(containerExtension)
        {
        }

        /// <summary>
        /// The get window token.
        /// </summary>
        /// <param name="dependencyObject">
        /// The dependency object.
        /// </param>
        /// <returns>
        /// The <see cref="WindowToken"/>.
        /// </returns>
        public static WindowToken GetWindowToken(DependencyObject dependencyObject)
        {
            return (WindowToken)dependencyObject.GetValue(WindowTokenProperty);
        }

        /// <summary>
        /// The set window token.
        /// </summary>
        /// <param name="dependencyObject">
        /// The dependency object.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public static void SetWindowToken(DependencyObject dependencyObject, WindowToken value)
        {
            dependencyObject.SetValue(WindowTokenProperty, value);
        }

        /// <summary>
        /// The show managed.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <param name="callback">
        /// The callback.
        /// </param>
        /// <returns>
        /// The <see cref="WindowToken"/>.
        /// </returns>
        internal WindowToken ShowManaged(string name, IDialogParameters parameters, Action<IDialogResult> callback)
        {
            this.Show(name, parameters, callback);
            return this.token;
        }

        /// <summary>
        /// The activate.
        /// </summary>
        /// <param name="token">
        /// The token.
        /// </param>
        internal void Activate(WindowToken token)
        {
            var dialogWindow = this.window[token];

            if (dialogWindow is Window w)
            {
                w.Activate();
            }
        }

        /// <summary>
        /// The close.
        /// </summary>
        /// <param name="token">
        /// The token.
        /// </param>
        internal void Close(WindowToken token)
        {
            var dialogWindow = this.window[token];
            dialogWindow.Close();
        }

        /// <summary>
        /// The configure dialog window events.
        /// </summary>
        /// <param name="dialogWindow">
        /// The dialog window.
        /// </param>
        /// <param name="callback">
        /// The callback.
        /// </param>
        protected override void ConfigureDialogWindowEvents(IDialogWindow dialogWindow, Action<IDialogResult> callback)
        {
            base.ConfigureDialogWindowEvents(dialogWindow, callback);

            EventHandler tokeCleanup = null;

            tokeCleanup = (s, e) =>
            {
                dialogWindow.Closed -= tokeCleanup;
                var token = GetWindowToken((DependencyObject)dialogWindow);
                this.window.Remove(token);
            };
            dialogWindow.Closed += tokeCleanup;
        }

        /// <summary>
        /// The create dialog window.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The <see cref="IDialogWindow"/>.
        /// </returns>
        protected override IDialogWindow CreateDialogWindow(string name)
        {
            var dialogWindow = base.CreateDialogWindow(name);

            if (dialogWindow is Window window)
            {
            }

            this.token = new WindowToken();

            SetWindowToken((DependencyObject)dialogWindow, this.token);

            this.window.Add(this.token, dialogWindow);

            return dialogWindow;
        }
    }
}
